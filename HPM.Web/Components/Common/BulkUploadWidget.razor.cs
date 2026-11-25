using System.Globalization;
using System.Text.RegularExpressions;
using HPM.Web.Data;
using HPM.Web.Data.Entities;
using HPM.Web.Security;
using HPM.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace HPM.Web.Components.Common;

public partial class BulkUploadWidget
{
    [Inject] private IDbContextFactory<HpmDbContext> DbFactory { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private HospitalLicenseService Licenses { get; set; } = default!;

    [Parameter, EditorRequired] public string TypeKey { get; set; } = "doctor";
    [Parameter] public EventCallback OnImported { get; set; }
    [Parameter] public bool ShowFullPageGuide { get; set; }

    private const long MaxFileBytes = 5 * 1024 * 1024;
    private const int MaxRows = 5000;
    private const int MedicineCategoryId = 3;

    private UploadType? selected;
    private HashSet<string> enabledModules = new(StringComparer.OrdinalIgnoreCase);
    private List<ImportRow> rows = [];
    private bool busy;
    private string? message;
    private string messageClass = "info";
    private string? lookupHint;
    private int inputKey;
    private ActiveModalKind activeModal = ActiveModalKind.None;
    private List<DependentInfo> dependentInfos = [];
    private int currentRecordCount;
    private bool progressActive;
    private int progressPercent;
    private string progressLabel = string.Empty;
    private bool isDragOver;
    private string? selectedFileName;

    private int ImportableCount => rows.Count(r => r.Status is RowStatus.New or RowStatus.Update);

    private string L(string bn, string en) => I18n.IsBangla ? bn : en;

    // ─────────────────────────── Upload type definitions ───────────────────────────

    private static readonly UploadType[] UploadTypes =
    [
        new("doctor", "Doctor", "ডাক্তার", "🩺", null,
            "Doctor profiles. Key: Name.", "ডাক্তারের প্রোফাইল। মিলানো হয় Name দিয়ে।",
            [
                new("Name", true, "Doctor's full name (max 100)", "Abdul Karim"),
                new("Title", false, "Prefix, e.g. Dr., Prof. Dr. (max 50)", "Dr."),
                new("Degree", false, "Degrees (max 150)", "MBBS, FCPS (Medicine)"),
                new("Department", false, "Must match an existing clinical department name", "Medicine"),
                new("Contact", false, "Phone (max 50)", "01711000000"),
                new("Email", false, "Email (max 50)", "doctor@example.com"),
                new("Room", false, "Chamber/room no. (max 10)", "204"),
                new("ConsultationTime", false, "Free text (max 100)", "5 PM - 9 PM"),
                new("OffDay", false, "Weekly off day", "Friday"),
                new("ConsultationFee", false, "Number", "800"),
                new("OpdConsultationFee", false, "Number", "500"),
                new("Priority", false, "Whole number (lower shows first)", "1"),
            ]),
        new("nurse", "Nurse", "নার্স", "👩‍⚕️", LicensedModules.Hrm,
            "Nurses as HRM employees (Nursing department, Staff Nurse designation by default). Key: EmployeeNo.",
            "নার্সদের HRM employee হিসেবে যোগ করা হবে (ডিফল্ট Nursing department ও Staff Nurse)। মিলানো হয় EmployeeNo দিয়ে।",
            [
                new("EmployeeNo", true, "Unique employee no. (max 30)", "N-1001"),
                new("Name", true, "Full name (max 150)", "Farzana Akter"),
                new("Gender", false, "Male / Female / Other", "Female"),
                new("Phone", false, "Max 30", "01711000000"),
                new("Email", false, "Max 150", "nurse@example.com"),
                new("Address", false, "Max 300", "Mirpur, Dhaka"),
                new("DateOfBirth", false, "yyyy-mm-dd or dd/mm/yyyy", "1995-04-12"),
                new("JoiningDate", false, "yyyy-mm-dd or dd/mm/yyyy (default today)", "2024-01-10"),
                new("EmploymentType", false, "Permanent / Contract / Probation / Part-time (default Permanent)", "Permanent"),
                new("BasicSalary", false, "Number", "25000"),
                new("AttendanceNo", false, "Unique attendance device no. (max 50)", "1001"),
                new("Department", false, "HRM department name or code (default Nursing)", "Nursing"),
                new("Designation", false, "HRM designation name or code (default Staff Nurse)", "Staff Nurse"),
            ]),
        new("medicine", "Pharmacy Medicine", "ফার্মেসি ঔষধ", "💊", LicensedModules.Pharmacy,
            "Medicine catalogue. Type, Generic and Supplier are created automatically if missing. Key: Name.",
            "ঔষধের তালিকা। Type, Generic ও Supplier না থাকলে নতুন তৈরি হবে। মিলানো হয় Name দিয়ে।",
            [
                new("Name", true, "Brand name with strength (max 200)", "Napa 500mg"),
                new("Type", false, "Dosage type", "Tablet"),
                new("Generic", false, "Generic name", "Paracetamol"),
                new("Supplier", false, "Supplier / company (max 50)", "Beximco Pharma"),
                new("BuyRate", false, "Number", "0.80"),
                new("SaleRate", true, "Number", "1.20"),
                new("ReOrderLevel", false, "Whole number", "100"),
                new("VatApplicable", false, "Yes / No", "No"),
            ]),
        new("canteen", "Canteen Food", "ক্যান্টিন খাবার", "🍽️", LicensedModules.Canteen,
            "Canteen food & products. Category is created automatically if missing. Key: Name.",
            "ক্যান্টিনের খাবার ও পণ্য। Category না থাকলে নতুন তৈরি হবে। মিলানো হয় Name দিয়ে।",
            [
                new("Name", true, "Item name (max 100)", "Chicken Biryani"),
                new("Category", true, "Category name (max 50)", "Rice Items"),
                new("SaleRate", true, "Number", "180"),
                new("BuyRate", false, "Number", "120"),
                new("Unit", false, "Max 50", "Plate"),
                new("ReorderLevel", false, "Whole number", "10"),
                new("OpeningStock", false, "Number — only used for new items", "0"),
                new("IsKitchenItem", false, "Yes / No", "Yes"),
            ]),
        new("pathology", "Pathology Investigations", "প্যাথোলজি ইনভেস্টিগেশন", "🔬", LicensedModules.Diagnostics,
            "Pathology investigation list. Category must already exist in Diagnostic Investigation Master. Key: InvestigationName + Category.",
            "প্যাথোলজি ইনভেস্টিগেশনের তালিকা। Category আগে থেকেই Diagnostic Investigation Master-এ থাকতে হবে। মিলানো হয় InvestigationName + Category দিয়ে।",
            DiagnosticColumns("CBC", "Haematology")),
        new("imaging", "Imaging Investigations", "ইমেজিং ইনভেস্টিগেশন", "IMG", LicensedModules.Diagnostics,
            "Imaging / radiology investigation list. Category must already exist in Diagnostic Investigation Master. Key: InvestigationName + Category.",
            "ইমেজিং / রেডিওলজি ইনভেস্টিগেশনের তালিকা। Category আগে থেকেই Diagnostic Investigation Master-এ থাকতে হবে। মিলানো হয় InvestigationName + Category দিয়ে।",
            DiagnosticColumns("X-Ray Chest PA View", "X-Ray")),
        new("procedure", "Special Investigations", "স্পেশাল ইনভেস্টিগেশন", "〰️", LicensedModules.Diagnostics,
            "ECG, Echo, EEG, ETT, Endoscopy & functional investigations. Category must already exist in Diagnostic Investigation Master. Key: InvestigationName + Category.",
            "ইসিজি, ইকো, ইইজি, ইটিটি ও এন্ডোস্কোপি ইনভেস্টিগেশন। Category আগে থেকেই Diagnostic Investigation Master-এ থাকতে হবে।",
            DiagnosticColumns("ECG 12 Lead", "ECG")),
    ];

    private static ColumnDef[] DiagnosticColumns(string exampleTest, string exampleCategory) =>
    [
        new("Code", false, "Investigation ID (blank for new; repeat same Code for multiple parameters)", ""),
        new("InvestigationName", true, "Investigation name (max 200)", exampleTest),
        new("Category", true, "Existing diagnostic category name", exampleCategory),
        new("Rate", true, "Number", "500"),
        new("DeliveryHours", false, "Report delivery time in hours (1-720)", "24"),
        new("ParameterName", false, "Optional report parameter name (one per row)", ""),
        new("Unit", false, "Parameter unit (e.g. g/dL, mg/dL)", ""),
        new("ReferenceRange", false, "Normal reference range", ""),
        new("SortOrder", false, "Parameter display order (1, 2, 3...)", "1"),
    ];

    // ─────────────────────────── Lifecycle ───────────────────────────

    protected override async Task OnInitializedAsync()
    {
        I18n.OnLanguageChanged += HandleLanguageChanged;
        enabledModules = await Licenses.EnabledModulesAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        var next = UploadTypes.FirstOrDefault(t => string.Equals(t.Key, TypeKey, StringComparison.OrdinalIgnoreCase) && IsLicensed(t))
                   ?? UploadTypes.FirstOrDefault(IsLicensed);
        if (next?.Key != selected?.Key)
        {
            selected = next;
            ResetUpload();
            await LoadLookupHintAsync();
        }
    }

    private bool IsLicensed(UploadType t) => t.Module is null || enabledModules.Contains(t.Module);

    private void HandleLanguageChanged() => InvokeAsync(StateHasChanged);

    public void Dispose() => I18n.OnLanguageChanged -= HandleLanguageChanged;

    private void ResetUpload()
    {
        rows = [];
        message = null;
        selectedFileName = null;
        isDragOver = false;
        inputKey++;
    }

    private void OpenModal(ActiveModalKind kind) => activeModal = kind;

    private void CloseModal()
    {
        if (busy || progressActive) return;
        activeModal = ActiveModalKind.None;
    }

    private async Task SetProgressAsync(int percent, string label)
    {
        progressActive = true;
        progressPercent = Math.Clamp(percent, 0, 100);
        progressLabel = label;
        await InvokeAsync(StateHasChanged);
        await Task.Delay(120);
    }

    private void ClearProgress()
    {
        progressActive = false;
        progressPercent = 0;
        progressLabel = string.Empty;
    }

    private async Task FixLookupAsync(ImportRow r, string column, string? newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue)) return;
        r.Set(column, newValue);
        await ValidateAsync();
    }

    /// <summary>Loads dependent master table counts, setup links, and current record count.</summary>
    private async Task LoadLookupHintAsync()
    {
        lookupHint = null;
        dependentInfos = [];
        currentRecordCount = 0;
        if (selected is null) return;
        await using var db = await DbFactory.CreateDbContextAsync();
        switch (selected.Key)
        {
            case "doctor":
                var depts = await db.Departments.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).Select(x => x.Name).ToListAsync();
                currentRecordCount = await db.Doctors.AsNoTracking().CountAsync();
                dependentInfos.Add(new("Departments", depts.Count, "/masters/departments", L("Department সেটআপ", "Manage Departments")));
                if (depts.Count > 0) lookupHint = L("সঠিক Department: ", "Valid Department values: ") + string.Join(", ", depts);
                break;
            case "nurse":
                var hd = await db.HrmDepartments.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).Select(x => x.Name).ToListAsync();
                var hg = await db.HrmDesignations.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).Select(x => x.Name).ToListAsync();
                currentRecordCount = await db.HrmEmployees.AsNoTracking().CountAsync();
                dependentInfos.Add(new("HrmDepartments", hd.Count, "/hrm", L("HRM সেটআপ", "Manage HRM")));
                dependentInfos.Add(new("HrmDesignations", hg.Count, "/hrm", L("HRM সেটআপ", "Manage HRM")));
                lookupHint = L("Department: ", "Departments: ") + string.Join(", ", hd) + " | " + L("Designation: ", "Designations: ") + string.Join(", ", hg);
                break;
            case "medicine":
                var mt = await db.MedicineTypes.AsNoTracking().CountAsync();
                var gg = await db.GenericGroups.AsNoTracking().CountAsync();
                var ps = await db.PharmacySuppliers.AsNoTracking().CountAsync();
                currentRecordCount = await db.Services.AsNoTracking().CountAsync(x => x.CategoryID == MedicineCategoryId);
                dependentInfos.Add(new("MedicineTypes", mt, "/pharmacy/masters", L("ফার্মেসি মাস্টার", "Pharmacy Masters")));
                dependentInfos.Add(new("GenericGroups", gg, "/pharmacy/masters", L("ফার্মেসি মাস্টার", "Pharmacy Masters")));
                dependentInfos.Add(new("PharmacySuppliers", ps, "/pharmacy/masters", L("ফার্মেসি মাস্টার", "Pharmacy Masters")));
                break;
            case "canteen":
                var cc = await db.CanteenCategories.AsNoTracking().CountAsync();
                currentRecordCount = await db.CanteenProducts.AsNoTracking().CountAsync();
                dependentInfos.Add(new("CanteenCategories", cc, "/canteen/products", L("ক্যান্টিন মাস্টার", "Manage Canteen")));
                break;
            case "pathology":
            case "imaging":
            case "procedure":
                var targetDiagType = selected.Key switch
                {
                    "pathology" => DiagnosticTestRouting.Pathology,
                    "procedure" => DiagnosticTestRouting.Procedure,
                    _ => DiagnosticTestRouting.Imaging
                };
                var allCats = (await db.ServiceCategories.AsNoTracking().ToListAsync()).Where(DiagnosticTestRouting.IsDiagnosticCategory).ToList();
                var cats = allCats
                    .Where(c => DiagnosticTestRouting.ResolveCategoryType(c) == targetDiagType)
                    .Select(c => c.Name).OrderBy(x => x).ToList();
                if (cats.Count == 0)
                    cats = allCats.Select(c => c.Name).OrderBy(x => x).ToList();
                var catIds = allCats.Select(c => c.ID).ToList();
                var diagTests = await db.Services.AsNoTracking().Where(x => x.CategoryID != null && catIds.Contains(x.CategoryID.Value)).ToListAsync();
                var catDict = allCats.ToDictionary(x => x.ID);
                currentRecordCount = diagTests.Count(t => t.CategoryID.HasValue && catDict.TryGetValue(t.CategoryID.Value, out var c)
                    && DiagnosticTestRouting.Resolve(t, c) == targetDiagType);
                dependentInfos.Add(new("ServiceCategories", allCats.Count, "/diagnostic/investigations", L("ইনভেস্টিগেশন মাস্টার", "Investigation Master")));
                lookupHint = L("সঠিক Category: ", "Valid Category values: ") + (cats.Count > 0 ? string.Join(", ", cats) : L("কোনো category নেই — আগে Diagnostic Test Master-এ তৈরি করুন", "none — create one in Diagnostic Test Master first"));
                break;
        }
    }

    // ─────────────────────────── Template & Export ───────────────────────────

    private Task DownloadTemplateAsync() => BuildAndDownloadWorkbookAsync(templateOnly: true);

    private Task ExportDataAsync() => BuildAndDownloadWorkbookAsync(templateOnly: false);

    private async Task BuildAndDownloadWorkbookAsync(bool templateOnly)
    {
        if (selected is null || progressActive) return;
        try
        {
            await SetProgressAsync(20, templateOnly ? L("নির্ভরশীল টেবিল লোড হচ্ছে...", "Loading dependent tables...") : L("ডেটাবেস থেকে রেকর্ড আনা হচ্ছে...", "Loading records..."));
            var headers = selected.Columns.Select(c => c.Name).ToList();
        var dataRows = new List<IReadOnlyList<string>>();
        var lookupSheets = new List<XlsxWorkbookBuilder.SheetSpec>();
        var validations = new List<XlsxWorkbookBuilder.DropdownValidation>();
        var mainSheetName = selected.Key switch
        {
            "doctor" => "Doctors",
            "nurse" => "Nurses",
            "medicine" => "Medicines",
            "canteen" => "CanteenFood",
            "pathology" => "PathologyInvestigations",
            "imaging" => "ImagingInvestigations",
            "procedure" => "SpecialInvestigations",
            _ => "Data"
        };

        await using var db = await DbFactory.CreateDbContextAsync();

        static string Dec(decimal? v) => v.HasValue ? v.Value.ToString(CultureInfo.InvariantCulture) : "";
        static string IntStr(int? v) => v.HasValue ? v.Value.ToString(CultureInfo.InvariantCulture) : "";

        switch (selected.Key)
        {
            case "doctor":
                var allDepts = await db.Departments.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
                var docDepts = allDepts.ToDictionary(x => x.ID, x => x.Name);
                var deptRows = allDepts.Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.Name))
                    .Select(x => (IReadOnlyList<string>)[x.Name.Trim(), x.ID.ToString(CultureInfo.InvariantCulture)]).ToList();
                lookupSheets.Add(new("Departments", ["DepartmentName", "ID"], deptRows));
                validations.Add(new(3, "Departments", deptRows.Count));

                var doctors = await db.Doctors.AsNoTracking().OrderBy(x => x.Priority ?? int.MaxValue).ThenBy(x => x.Name).ToListAsync();
                foreach (var d in doctors)
                {
                    dataRows.Add([
                        d.Name ?? "",
                        d.Title ?? "",
                        d.Degree ?? "",
                        d.Department.HasValue && docDepts.TryGetValue(d.Department.Value, out var dn) ? dn : "",
                        d.Contact ?? "",
                        d.Email ?? "",
                        d.Room ?? "",
                        d.ConsultationTime ?? "",
                        d.OffDay ?? "",
                        Dec(d.ConsultationFee),
                        Dec(d.OpdConsultationFee),
                        IntStr(d.Priority)
                    ]);
                }
                break;

            case "nurse":
                var allHrmDepts = await db.HrmDepartments.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
                var allHrmDesigs = await db.HrmDesignations.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
                var hrmDepts = allHrmDepts.ToDictionary(x => x.ID, x => x);
                var hrmDesigs = allHrmDesigs.ToDictionary(x => x.ID, x => x);

                var hrmDeptRows = allHrmDepts.Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .Select(x => (IReadOnlyList<string>)[x.Name.Trim(), x.Code, x.ID.ToString(CultureInfo.InvariantCulture)]).ToList();
                var hrmDesigRows = allHrmDesigs.Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .Select(x => (IReadOnlyList<string>)[x.Name.Trim(), x.Code, x.ID.ToString(CultureInfo.InvariantCulture)]).ToList();
                lookupSheets.Add(new("HrmDepartments", ["DepartmentName", "Code", "ID"], hrmDeptRows));
                lookupSheets.Add(new("HrmDesignations", ["DesignationName", "Code", "ID"], hrmDesigRows));
                validations.Add(new(11, "HrmDepartments", hrmDeptRows.Count));
                validations.Add(new(12, "HrmDesignations", hrmDesigRows.Count));

                var emps = await db.HrmEmployees.AsNoTracking().OrderBy(x => x.EmployeeNo).ToListAsync();
                foreach (var emp in emps)
                {
                    hrmDepts.TryGetValue(emp.DepartmentID, out var dept);
                    hrmDesigs.TryGetValue(emp.DesignationID, out var desig);
                    var isNurse = (dept is not null && (string.Equals(dept.Code, "NURS", StringComparison.OrdinalIgnoreCase) || dept.Name.Contains("Nurs", StringComparison.OrdinalIgnoreCase)))
                                  || (desig is not null && (string.Equals(desig.Code, "NURSE", StringComparison.OrdinalIgnoreCase) || desig.Name.Contains("Nurse", StringComparison.OrdinalIgnoreCase)));
                    if (!isNurse) continue;

                    dataRows.Add([
                        emp.EmployeeNo,
                        emp.Name,
                        emp.Gender ?? "",
                        emp.Phone ?? "",
                        emp.Email ?? "",
                        emp.Address ?? "",
                        emp.DateOfBirth?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "",
                        emp.JoiningDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        emp.EmploymentType ?? "",
                        Dec(emp.BasicSalary),
                        emp.AttendanceNo ?? "",
                        dept?.Name ?? "",
                        desig?.Name ?? ""
                    ]);
                }
                break;

            case "medicine":
                var allTypes = await db.MedicineTypes.AsNoTracking().OrderBy(x => x.Type).ToListAsync();
                var allGenerics = await db.GenericGroups.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
                var allSuppliers = await db.PharmacySuppliers.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
                var medTypes = allTypes.ToDictionary(x => x.ID, x => x.Type);
                var generics = allGenerics.ToDictionary(x => x.ID, x => x.Name ?? "");
                var suppliers = allSuppliers.ToDictionary(x => x.ID, x => x.Name ?? "");

                var typeRows = allTypes.Where(x => !string.IsNullOrWhiteSpace(x.Type))
                    .Select(x => (IReadOnlyList<string>)[x.Type.Trim(), x.ID.ToString(CultureInfo.InvariantCulture)]).ToList();
                var genRows = allGenerics.Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .Select(x => (IReadOnlyList<string>)[x.Name!.Trim(), x.ID.ToString(CultureInfo.InvariantCulture)]).ToList();
                var supRows = allSuppliers.Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .Select(x => (IReadOnlyList<string>)[x.Name!.Trim(), x.ID.ToString(CultureInfo.InvariantCulture)]).ToList();
                lookupSheets.Add(new("MedicineTypes", ["TypeName", "ID"], typeRows));
                lookupSheets.Add(new("GenericGroups", ["GenericName", "ID"], genRows));
                lookupSheets.Add(new("PharmacySuppliers", ["SupplierName", "ID"], supRows));
                validations.Add(new(1, "MedicineTypes", typeRows.Count));
                validations.Add(new(2, "GenericGroups", genRows.Count));
                validations.Add(new(3, "PharmacySuppliers", supRows.Count));

                var meds = await db.Services.AsNoTracking().Where(x => x.CategoryID == MedicineCategoryId).OrderBy(x => x.Name).ToListAsync();
                foreach (var m in meds)
                {
                    dataRows.Add([
                        m.Name ?? "",
                        m.TypeID.HasValue && medTypes.TryGetValue(m.TypeID.Value, out var tn) ? tn : "",
                        m.GenericID.HasValue && generics.TryGetValue(m.GenericID.Value, out var gn) ? gn : "",
                        m.SupplierID.HasValue && suppliers.TryGetValue(m.SupplierID.Value, out var sn) ? sn : "",
                        Dec(m.BuyRate),
                        Dec(m.SaleRate),
                        IntStr(m.ReOrderLevel),
                        m.IsVATApp == true ? "Yes" : "No"
                    ]);
                }
                break;

            case "canteen":
                var allCanteenCats = await db.CanteenCategories.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
                var canteenCats = allCanteenCats.ToDictionary(x => x.ID, x => x.Name);
                var canteenCatRows = allCanteenCats.Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .Select(x => (IReadOnlyList<string>)[x.Name.Trim(), x.ID.ToString(CultureInfo.InvariantCulture)]).ToList();
                lookupSheets.Add(new("CanteenCategories", ["CategoryName", "ID"], canteenCatRows));
                validations.Add(new(1, "CanteenCategories", canteenCatRows.Count));

                var items = await db.CanteenProducts.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
                foreach (var item in items)
                {
                    dataRows.Add([
                        item.Name,
                        canteenCats.TryGetValue(item.CategoryID, out var cn) ? cn : "",
                        Dec(item.SaleRate),
                        Dec(item.BuyRate),
                        item.Unit ?? "",
                        IntStr(item.ReorderLevel),
                        Dec(item.Balance),
                        item.IsKitchenItem == true ? "Yes" : "No"
                    ]);
                }
                break;

            case "pathology":
            case "imaging":
            case "procedure":
                var exportDiagType = selected.Key switch
                {
                    "pathology" => DiagnosticTestRouting.Pathology,
                    "procedure" => DiagnosticTestRouting.Procedure,
                    _ => DiagnosticTestRouting.Imaging
                };
                var diagCats = (await db.ServiceCategories.AsNoTracking().OrderBy(x => x.Name).ToListAsync())
                    .Where(DiagnosticTestRouting.IsDiagnosticCategory)
                    .ToDictionary(x => x.ID, x => x);
                var filteredDiagCats = diagCats.Values.Where(c => DiagnosticTestRouting.ResolveCategoryType(c) == exportDiagType).ToList();
                if (filteredDiagCats.Count == 0) filteredDiagCats = diagCats.Values.ToList();
                var diagCatRows = filteredDiagCats.Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .Select(x => (IReadOnlyList<string>)[x.Name.Trim(), x.ID.ToString(CultureInfo.InvariantCulture)]).ToList();
                lookupSheets.Add(new("ServiceCategories", ["CategoryName", "ID"], diagCatRows));
                validations.Add(new(2, "ServiceCategories", diagCatRows.Count));

                var catIds = diagCats.Keys.ToList();
                var tests = await db.Services.AsNoTracking()
                    .Where(x => x.CategoryID != null && catIds.Contains(x.CategoryID.Value))
                    .OrderBy(x => x.Name)
                    .ToListAsync();
                var testIds = tests.Select(t => t.ID).ToHashSet();
                var paramsByService = (await db.LabTestParameters.AsNoTracking()
                    .Where(p => testIds.Contains(p.ServiceID))
                    .OrderBy(p => p.ServiceID)
                    .ThenBy(p => p.SortOrder)
                    .ToListAsync())
                    .GroupBy(p => p.ServiceID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var t in tests)
                {
                    if (!t.CategoryID.HasValue || !diagCats.TryGetValue(t.CategoryID.Value, out var cat)) continue;
                    if (DiagnosticTestRouting.Resolve(t, cat) != exportDiagType) continue;

                    if (paramsByService.TryGetValue(t.ID, out var svcParams) && svcParams.Count > 0)
                    {
                        for (var idx = 0; idx < svcParams.Count; idx++)
                        {
                            var p = svcParams[idx];
                            var isFirst = idx == 0;
                            dataRows.Add([
                                DiagnosticTestRouting.FormatCode(t, cat),
                                isFirst ? (t.Name ?? "") : "",
                                isFirst ? cat.Name : "",
                                isFirst ? Dec(t.SaleRate) : "",
                                isFirst ? IntStr(t.DeliveryHours) : "",
                                p.ParameterName,
                                p.Unit ?? "",
                                p.ReferenceRange ?? "",
                                p.SortOrder.ToString(CultureInfo.InvariantCulture)
                            ]);
                        }
                    }
                    else
                    {
                        dataRows.Add([
                            DiagnosticTestRouting.FormatCode(t, cat),
                            t.Name ?? "",
                            cat.Name,
                            Dec(t.SaleRate),
                            IntStr(t.DeliveryHours),
                            "",
                            "",
                            "",
                            ""
                        ]);
                    }
                }
                break;
        }

        if (templateOnly)
        {
            var sheetByName = lookupSheets.ToDictionary(s => s.Name, s => s, StringComparer.OrdinalIgnoreCase);
            var valByCol = validations.ToDictionary(v => v.ColumnIndex, v => v);
            var dummyRow = new string[selected.Columns.Length];
            for (var c = 0; c < selected.Columns.Length; c++)
            {
                if (valByCol.TryGetValue(c, out var v)
                    && sheetByName.TryGetValue(v.TargetSheetName, out var targetSheet)
                    && targetSheet.Rows.Count > 0)
                {
                    dummyRow[c] = targetSheet.Rows[0][0];
                }
                else
                {
                    dummyRow[c] = selected.Columns[c].Example;
                }
            }
            dataRows = [dummyRow];
        }

        var sheets = new List<XlsxWorkbookBuilder.SheetSpec>(1 + lookupSheets.Count)
        {
            new(mainSheetName, headers, dataRows, validations)
        };
        sheets.AddRange(lookupSheets);

            await SetProgressAsync(65, L("Excel ফাইল তৈরি হচ্ছে...", "Generating Excel file..."));
            var xlsx = XlsxWorkbookBuilder.Build(sheets);
            var downloadName = templateOnly
                ? $"{selected.Key}-template.xlsx"
                : $"{selected.Key}-export-{DateTime.Today:yyyyMMdd}.xlsx";
            await SetProgressAsync(100, L("ডাউনলোড প্রস্তুত", "Download ready"));
            await JS.InvokeVoidAsync("hpm.downloadFile",
                downloadName,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Convert.ToBase64String(xlsx));
            await Task.Delay(250);
            activeModal = ActiveModalKind.None;
            await OnImported.InvokeAsync();
        }
        finally
        {
            ClearProgress();
        }
    }

    // ─────────────────────────── File parsing ───────────────────────────

    private async Task OnFileSelectedAsync(InputFileChangeEventArgs e)
    {
        isDragOver = false;
        if (selected is null) return;
        rows = [];
        message = null;
        var file = e.File;
        selectedFileName = file.Name;
        if (file.Size > MaxFileBytes)
        {
            Fail(L("ফাইল ৫ MB-এর বেশি হতে পারবে না।", "File must be 5 MB or smaller."));
            return;
        }

        busy = true;
        try
        {
            await SetProgressAsync(25, L("ফাইল পড়া হচ্ছে...", "Reading file..."));
            using var ms = new MemoryStream();
            await file.OpenReadStream(MaxFileBytes).CopyToAsync(ms);
            var table = TabularFileReader.Read(file.Name, ms.ToArray());
            if (table.Count < 2)
            {
                Fail(L("ফাইলে header-এর নিচে কোনো ডেটা নেই।", "The file has no data rows under the header."));
                return;
            }
            if (table.Count - 1 > MaxRows)
            {
                Fail(L($"একবারে সর্বোচ্চ {MaxRows}টি row আপলোড করা যাবে।", $"A maximum of {MaxRows} rows can be uploaded at once."));
                return;
            }

            var header = table[0].Select(Normalize).ToArray();
            var missing = selected.Columns.Where(c => c.Required && !header.Contains(Normalize(c.Name))).Select(c => c.Name).ToList();
            if (missing.Count > 0)
            {
                Fail(L("আবশ্যক column পাওয়া যায়নি: ", "Required columns are missing: ") + string.Join(", ", missing) + L(" — Template ব্যবহার করুন।", " — please use the template."));
                return;
            }

            rows = table.Skip(1).Select((cells, i) =>
            {
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var c = 0; c < header.Length && c < cells.Length; c++)
                    if (!string.IsNullOrEmpty(header[c])) values[header[c]] = cells[c]?.Trim() ?? string.Empty;
                return new ImportRow(i + 2, values);
            }).ToList();

            await SetProgressAsync(75, L($"{rows.Count}টি row যাচাই হচ্ছে...", $"Validating {rows.Count} rows..."));
            await ValidateAsync();
            await SetProgressAsync(100, L("যাচাই সম্পন্ন", "Validation complete"));
        }
        catch (Exception ex)
        {
            Fail(L("ফাইল পড়া যায়নি: ", "Could not read the file: ") + ex.Message);
        }
        finally
        {
            ClearProgress();
            busy = false;
        }
    }

    private static string Normalize(string? header) => Regex.Replace(header ?? string.Empty, "[^A-Za-z0-9]", "").ToLowerInvariant();

    private async Task ValidateAsync()
    {
        await using var db = await DbFactory.CreateDbContextAsync();
        await ProcessAsync(db, commit: false);
        var errors = rows.Count(r => r.Status == RowStatus.Error);
        message = errors > 0
            ? L($"{errors}টি row-তে ভুল আছে — ঠিক করে আবার আপলোড করুন, অথবা বাকি সঠিক row গুলো আপলোড করুন।", $"{errors} rows have errors — fix and re-upload, or import only the valid rows.")
            : L("ফাইল যাচাই হয়েছে। নিচে দেখে আপলোড করুন।", "File checked. Review below and import.");
        messageClass = errors > 0 ? "warn" : "info";
    }

    private async Task ImportAsync()
    {
        if (selected is null || ImportableCount == 0) return;
        busy = true;
        try
        {
            await SetProgressAsync(20, L("ডেটা প্রস্তুত হচ্ছে...", "Preparing import..."));
            var totalToImport = ImportableCount;
            await SetProgressAsync(60, L($"{totalToImport}টি রেকর্ড সেভ হচ্ছে...", $"Saving {totalToImport} records..."));
            var (added, updated) = await ResilientTransaction.ExecuteAsync(DbFactory, async db =>
            {
                await ProcessAsync(db, commit: true);
                return (rows.Count(r => r.Status == RowStatus.New), rows.Count(r => r.Status == RowStatus.Update));
            });
            var skipped = rows.Count(r => r.Status is RowStatus.Skip or RowStatus.Error);
            rows = [];
            selectedFileName = null;
            inputKey++;
            await LoadLookupHintAsync();
            await SetProgressAsync(100, L("আপলোড সম্পন্ন", "Import complete"));
            message = L($"সফল! নতুন {added}টি, update {updated}টি, বাদ {skipped}টি।", $"Done! {added} added, {updated} updated, {skipped} skipped.");
            messageClass = "success";
            activeModal = ActiveModalKind.None;
            await OnImported.InvokeAsync();
        }
        catch (Exception ex)
        {
            Fail(L("আপলোড ব্যর্থ হয়েছে, কোনো ডেটা সেভ হয়নি: ", "Import failed, nothing was saved: ") + (ex.InnerException?.Message ?? ex.Message));
        }
        finally
        {
            ClearProgress();
            busy = false;
        }
    }

    private void Fail(string text)
    {
        message = text;
        messageClass = "error";
    }

    private string StatusText(RowStatus s) => s switch
    {
        RowStatus.New => L("নতুন", "New"),
        RowStatus.Update => L("Update", "Update"),
        RowStatus.Skip => L("Skip", "Skip"),
        _ => L("ভুল", "Error")
    };

    // ─────────────────────────── Validation + import ───────────────────────────
    // Each processor runs twice: once with commit=false to build the preview, and once
    // with commit=true inside a transaction to write the rows. Rows are re-validated on commit.

    private async Task ProcessAsync(HpmDbContext db, bool commit)
    {
        foreach (var r in rows) r.Reset();
        switch (selected!.Key)
        {
            case "doctor": await ProcessDoctorsAsync(db, commit); break;
            case "nurse": await ProcessNursesAsync(db, commit); break;
            case "medicine": await ProcessMedicinesAsync(db, commit); break;
            case "canteen": await ProcessCanteenAsync(db, commit); break;
            case "pathology": await ProcessDiagnosticAsync(db, commit, DiagnosticTestRouting.Pathology); break;
            case "imaging": await ProcessDiagnosticAsync(db, commit, DiagnosticTestRouting.Imaging); break;
            case "procedure": await ProcessDiagnosticAsync(db, commit, DiagnosticTestRouting.Procedure); break;
        }
        if (commit)
        {
            await db.SaveChangesAsync();
            HospitalMasterCacheService.InvalidateGlobalCache();
        }
    }

    /// <summary>Marks a row as New or Update depending on whether the key already exists.</summary>
    private bool ResolveExisting(ImportRow r, bool exists, HashSet<string> seenKeys, string key)
    {
        if (!seenKeys.Add(key))
        {
            r.Error(L("ফাইলে একই record একাধিকবার আছে", "Duplicate of an earlier row in this file"));
            return false;
        }
        if (r.HasErrors) return false;
        r.Status = exists ? RowStatus.Update : RowStatus.New;
        return true;
    }

    private async Task ProcessDoctorsAsync(HpmDbContext db, bool commit)
    {
        var departments = (await db.Departments.AsNoTracking().ToListAsync())
            .GroupBy(x => x.Name.Trim(), StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First().ID, StringComparer.OrdinalIgnoreCase);
        var existing = (await db.Doctors.ToListAsync())
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .GroupBy(x => x.Name!.Trim(), StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var r in rows)
        {
            var name = r.Required("Name", 100);
            r.Label = name;
            r.MaxLen("Title", 50); r.MaxLen("Degree", 150); r.MaxLen("Contact", 50); r.MaxLen("Email", 50); r.MaxLen("Room", 10); r.MaxLen("ConsultationTime", 100); r.MaxLen("OffDay", 100);
            var fee = r.Decimal("ConsultationFee");
            var opdFee = r.Decimal("OpdConsultationFee");
            var priority = r.Int("Priority");
            int? deptId = null;
            var deptName = r.Get("Department");
            if (deptName.Length > 0)
            {
                if (departments.TryGetValue(deptName, out var id)) deptId = id;
                else
                {
                    r.Error(L($"Department '{deptName}' পাওয়া যায়নি", $"Department '{deptName}' not found"));
                    r.FixableLookups.Add(("Department", deptName, departments.Keys.OrderBy(x => x).ToList()));
                }
            }

            existing.TryGetValue(name, out var doctor);
            if (!ResolveExisting(r, doctor is not null, seen, name) || !commit) continue;

            if (doctor is null)
            {
                doctor = new DoctorsProfile { Name = name, IsActive = true, EnteredBy = 1, TransDate = DateTime.Now };
                db.Doctors.Add(doctor);
            }
            doctor.Title = r.Optional("Title") ?? doctor.Title;
            doctor.Degree = r.Optional("Degree") ?? doctor.Degree;
            doctor.Department = deptId ?? doctor.Department;
            doctor.Contact = r.Optional("Contact") ?? doctor.Contact;
            doctor.Email = r.Optional("Email") ?? doctor.Email;
            doctor.Room = r.Optional("Room") ?? doctor.Room;
            doctor.ConsultationTime = r.Optional("ConsultationTime") ?? doctor.ConsultationTime;
            doctor.OffDay = r.Optional("OffDay") ?? doctor.OffDay;
            doctor.ConsultationFee = fee ?? doctor.ConsultationFee;
            doctor.OpdConsultationFee = opdFee ?? doctor.OpdConsultationFee;
            doctor.Priority = priority ?? doctor.Priority;
        }
    }

    private async Task ProcessNursesAsync(HpmDbContext db, bool commit)
    {
        var departments = await db.HrmDepartments.AsNoTracking().ToListAsync();
        var designations = await db.HrmDesignations.AsNoTracking().ToListAsync();
        var employees = await db.HrmEmployees.ToListAsync();
        var byNo = employees.ToDictionary(x => x.EmployeeNo.Trim(), StringComparer.OrdinalIgnoreCase);
        var attendanceOwners = employees.Where(x => !string.IsNullOrWhiteSpace(x.AttendanceNo))
            .ToDictionary(x => x.AttendanceNo!.Trim(), x => x.EmployeeNo, StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var seenAttendance = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        static T? FindByNameOrCode<T>(List<T> list, string value, Func<T, string> name, Func<T, string> code) where T : class =>
            list.FirstOrDefault(x => string.Equals(name(x), value, StringComparison.OrdinalIgnoreCase) || string.Equals(code(x), value, StringComparison.OrdinalIgnoreCase));

        foreach (var r in rows)
        {
            var empNo = r.Required("EmployeeNo", 30);
            var name = r.Required("Name", 150);
            r.Label = $"{empNo} — {name}";
            r.MaxLen("Gender", 20); r.MaxLen("Phone", 30); r.MaxLen("Email", 150); r.MaxLen("Address", 300); r.MaxLen("EmploymentType", 30);
            var attendanceNo = r.Optional("AttendanceNo");
            r.MaxLen("AttendanceNo", 50);
            var dob = r.Date("DateOfBirth");
            var joining = r.Date("JoiningDate");
            var salary = r.Decimal("BasicSalary");

            var deptValue = r.Optional("Department") ?? "NURS";
            var dept = FindByNameOrCode(departments, deptValue, x => x.Name, x => x.Code);
            if (dept is null)
            {
                r.Error(L($"Department '{deptValue}' পাওয়া যায়নি", $"Department '{deptValue}' not found"));
                r.FixableLookups.Add(("Department", deptValue, departments.Select(x => x.Name).OrderBy(x => x).ToList()));
            }
            var desigValue = r.Optional("Designation") ?? "NURSE";
            var desig = FindByNameOrCode(designations, desigValue, x => x.Name, x => x.Code);
            if (desig is null)
            {
                r.Error(L($"Designation '{desigValue}' পাওয়া যায়নি", $"Designation '{desigValue}' not found"));
                r.FixableLookups.Add(("Designation", desigValue, designations.Select(x => x.Name).OrderBy(x => x).ToList()));
            }

            if (attendanceNo is not null)
            {
                if (!seenAttendance.Add(attendanceNo))
                    r.Error(L("AttendanceNo ফাইলে একাধিকবার আছে", "AttendanceNo repeats in this file"));
                else if (attendanceOwners.TryGetValue(attendanceNo, out var owner) && !string.Equals(owner, empNo, StringComparison.OrdinalIgnoreCase))
                    r.Error(L($"AttendanceNo অন্য employee ({owner})-এর", $"AttendanceNo belongs to employee {owner}"));
            }

            byNo.TryGetValue(empNo, out var emp);
            if (!ResolveExisting(r, emp is not null, seen, empNo) || !commit) continue;

            if (emp is null)
            {
                emp = new HrmEmployee { EmployeeNo = empNo, CreatedAt = DateTime.UtcNow, Status = "Active", JoiningDate = joining ?? DateTime.Today };
                db.HrmEmployees.Add(emp);
            }
            emp.Name = name;
            emp.Gender = r.Optional("Gender") ?? emp.Gender;
            emp.Phone = r.Optional("Phone") ?? emp.Phone;
            emp.Email = r.Optional("Email") ?? emp.Email;
            emp.Address = r.Optional("Address") ?? emp.Address;
            emp.DateOfBirth = dob ?? emp.DateOfBirth;
            emp.JoiningDate = joining ?? emp.JoiningDate;
            emp.EmploymentType = r.Optional("EmploymentType") ?? emp.EmploymentType;
            emp.BasicSalary = salary ?? emp.BasicSalary;
            emp.AttendanceNo = attendanceNo ?? emp.AttendanceNo;
            emp.DepartmentID = dept!.ID;
            emp.DesignationID = desig!.ID;
            emp.UpdatedAt = DateTime.UtcNow;
        }
    }

    private async Task ProcessMedicinesAsync(HpmDbContext db, bool commit)
    {
        var types = (await db.MedicineTypes.ToListAsync()).GroupBy(x => x.Type.Trim(), StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        var generics = (await db.GenericGroups.ToListAsync()).Where(x => !string.IsNullOrWhiteSpace(x.Name)).GroupBy(x => x.Name!.Trim(), StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        var suppliers = (await db.PharmacySuppliers.ToListAsync()).Where(x => !string.IsNullOrWhiteSpace(x.Name)).GroupBy(x => x.Name!.Trim(), StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        var existing = (await db.Services.Where(x => x.CategoryID == MedicineCategoryId).ToListAsync())
            .Where(x => !string.IsNullOrWhiteSpace(x.Name)).GroupBy(x => x.Name!.Trim(), StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var toWrite = new List<(ImportRow Row, Service? Existing, string Name)>();

        foreach (var r in rows)
        {
            var name = r.Required("Name", 200);
            r.Label = name;
            r.MaxLen("Type", 100); r.MaxLen("Generic", 200); r.MaxLen("Supplier", 50);
            var buy = r.Decimal("BuyRate");
            var sale = r.Decimal("SaleRate", required: true);
            var reorder = r.Int("ReOrderLevel");
            r.Bool("VatApplicable");
            foreach (var (col, known) in new (string, Func<string, bool>)[] { ("Type", types.ContainsKey), ("Generic", generics.ContainsKey), ("Supplier", suppliers.ContainsKey) })
            {
                var v = r.Optional(col);
                if (v is not null && !known(v))
                    r.Messages.Add(L($"নতুন {col} তৈরি হবে: {v}", $"New {col} will be created: {v}"));
            }
            existing.TryGetValue(name, out var med);
            if (ResolveExisting(r, med is not null, seen, name)) toWrite.Add((r, med, name));
        }
        if (!commit || toWrite.Count == 0) return;

        // Create missing masters first so their identity IDs exist.
        foreach (var (r, _, _) in toWrite)
        {
            if (r.Optional("Type") is { } t && !types.ContainsKey(t)) { var e = new MedicineType { Type = t, EnteredBy = 1, TransDate = DateTime.Now }; db.MedicineTypes.Add(e); types[t] = e; }
            if (r.Optional("Generic") is { } g && !generics.ContainsKey(g)) { var e = new GenericGroup { Name = g, EnteredBy = 1, TransDate = DateTime.Now }; db.GenericGroups.Add(e); generics[g] = e; }
            if (r.Optional("Supplier") is { } s && !suppliers.ContainsKey(s)) { var e = new SupplierPharmacy { Name = s, TransDate = DateTime.Now }; db.PharmacySuppliers.Add(e); suppliers[s] = e; }
        }
        await db.SaveChangesAsync();

        foreach (var (r, med, name) in toWrite)
        {
            var entity = med ?? new Service { Name = name, CategoryID = MedicineCategoryId, InvestigationModuleID = 0 };
            if (med is null) db.Services.Add(entity);
            if (r.Optional("Type") is { } t) entity.TypeID = types[t].ID;
            if (r.Optional("Generic") is { } g) entity.GenericID = generics[g].ID;
            if (r.Optional("Supplier") is { } s) entity.SupplierID = suppliers[s].ID;
            entity.BuyRate = r.Decimal("BuyRate") ?? entity.BuyRate;
            entity.SaleRate = r.Decimal("SaleRate") ?? entity.SaleRate;
            entity.ReOrderLevel = r.Int("ReOrderLevel") ?? entity.ReOrderLevel;
            entity.IsVATApp = r.Bool("VatApplicable") ?? entity.IsVATApp ?? false;
        }
    }

    private async Task ProcessCanteenAsync(HpmDbContext db, bool commit)
    {
        var categories = (await db.CanteenCategories.ToListAsync()).GroupBy(x => x.Name.Trim(), StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        var existing = (await db.CanteenProducts.ToListAsync()).GroupBy(x => x.Name.Trim(), StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var toWrite = new List<(ImportRow Row, CanteenProduct? Existing, string Name, string Category)>();

        foreach (var r in rows)
        {
            var name = r.Required("Name", 100);
            var category = r.Required("Category", 50);
            r.Label = name;
            r.MaxLen("Unit", 50);
            r.Decimal("SaleRate", required: true);
            r.Decimal("BuyRate");
            r.Int("ReorderLevel");
            r.Decimal("OpeningStock");
            r.Bool("IsKitchenItem");
            if (category.Length > 0 && !categories.ContainsKey(category))
                r.Messages.Add(L($"নতুন Category তৈরি হবে: {category}", $"New category will be created: {category}"));
            existing.TryGetValue(name, out var product);
            if (ResolveExisting(r, product is not null, seen, name)) toWrite.Add((r, product, name, category));
        }
        if (!commit || toWrite.Count == 0) return;

        foreach (var (_, _, _, category) in toWrite)
        {
            if (!categories.ContainsKey(category)) { var c = new CanteenCategory { Name = category }; db.CanteenCategories.Add(c); categories[category] = c; }
        }
        await db.SaveChangesAsync();

        foreach (var (r, product, name, category) in toWrite)
        {
            var entity = product ?? new CanteenProduct { Name = name, Balance = r.Decimal("OpeningStock") ?? 0 };
            if (product is null) db.CanteenProducts.Add(entity);
            entity.CategoryID = categories[category].ID;
            entity.SaleRate = r.Decimal("SaleRate") ?? entity.SaleRate;
            entity.BuyRate = r.Decimal("BuyRate") ?? entity.BuyRate;
            entity.Unit = r.Optional("Unit") ?? entity.Unit;
            entity.ReorderLevel = r.Int("ReorderLevel") ?? entity.ReorderLevel;
            entity.IsKitchenItem = r.Bool("IsKitchenItem") ?? entity.IsKitchenItem;
        }
    }

    private async Task ProcessDiagnosticAsync(HpmDbContext db, bool commit, string diagnosticType)
    {
        var allDiagCats = (await db.ServiceCategories.AsNoTracking().ToListAsync())
            .Where(DiagnosticTestRouting.IsDiagnosticCategory)
            .ToList();
        var categories = allDiagCats
            .GroupBy(x => x.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().ID, StringComparer.OrdinalIgnoreCase);
        var categoryNamesById = allDiagCats.ToDictionary(x => x.ID, x => x.Name);
        var categoryIds = categories.Values.ToList();
        var existingServices = await db.Services.Where(x => x.CategoryID != null && categoryIds.Contains(x.CategoryID.Value)).ToListAsync();
        var existingById = existingServices.ToDictionary(x => x.ID);
        var existingByName = existingServices
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .GroupBy(x => $"{x.CategoryID}|{x.Name!.Trim()}", StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var seenHeaderByCode = new Dictionary<int, (string Name, string CatName, int CatId)>();
        var groupedTargets = new Dictionary<string, Service>(StringComparer.OrdinalIgnoreCase);
        var pendingParams = new List<(string Key, string ParamName, string? Unit, string? Range, int SortOrder)>();

        foreach (var r in rows)
        {
            var rawCode = r.Get("Code").Trim();
            int? code = null;
            if (rawCode.Length > 0)
            {
                if (DiagnosticTestRouting.TryParseCode(rawCode, out var parsedCode))
                    code = parsedCode;
                else
                    r.Error(L("Code অবশ্যই সঠিক ফরম্যাটে হতে হবে (যেমন PAT-10001 বা 10001)", "Code must be a valid ID or prefixed code (e.g. PAT-10001)"));
            }

            Service? dbMatchByCode = code.HasValue && existingById.TryGetValue(code.Value, out var foundByCode) ? foundByCode : null;
            (string Name, string CatName, int CatId) seenInfo = default;
            var hasSeenCodeInFile = code.HasValue && seenHeaderByCode.TryGetValue(code.Value, out seenInfo);
            var isContinuationRow = hasSeenCodeInFile || dbMatchByCode is not null;

            var name = r.Get("InvestigationName");
            if (name.Length == 0) name = r.Get("TestName");
            if (name.Length == 0 && hasSeenCodeInFile) name = seenInfo.Name;
            if (name.Length == 0 && dbMatchByCode is not null) name = dbMatchByCode.Name ?? string.Empty;
            if (name.Length == 0) r.Error("InvestigationName is required");
            else if (name.Length > 200) r.Error("InvestigationName is longer than 200 characters");

            var categoryName = r.Get("Category");
            int categoryId = 0;
            if (categoryName.Length == 0 && hasSeenCodeInFile)
            {
                categoryName = seenInfo.CatName;
                categoryId = seenInfo.CatId;
            }
            else if (categoryName.Length == 0 && dbMatchByCode?.CategoryID is int dbCatId && categoryNamesById.TryGetValue(dbCatId, out var dbCatName))
            {
                categoryName = dbCatName;
                categoryId = dbCatId;
            }
            else if (categoryName.Length == 0)
            {
                r.Error("Category is required");
            }
            else if (categoryName.Length > 200)
            {
                r.Error("Category is longer than 200 characters");
            }
            else if (!categories.TryGetValue(categoryName, out categoryId))
            {
                r.Error(L($"Diagnostic Category '{categoryName}' পাওয়া যায়নি", $"Diagnostic category '{categoryName}' not found"));
                r.FixableLookups.Add(("Category", categoryName, categories.Keys.OrderBy(x => x).ToList()));
            }

            var paramName = r.Get("ParameterName");
            if (paramName.Length > 200) r.Error("ParameterName is longer than 200 characters");

            r.Label = categoryName.Length > 0
                ? (paramName.Length > 0 ? $"{name} ({categoryName}) → {paramName}" : $"{name} ({categoryName})")
                : name;

            decimal? rate = null;
            var rawRate = r.Get("Rate");
            if (rawRate.Length > 0)
                rate = r.Decimal("Rate", required: true);
            else if (!isContinuationRow)
                rate = r.Decimal("Rate", required: true);

            var hours = r.Int("DeliveryHours");
            if (hours is < 1 or > 720) r.Error(L("DeliveryHours ১ থেকে ৭২০ এর মধ্যে হতে হবে", "DeliveryHours must be between 1 and 720"));
            var sortOrder = r.Int("SortOrder") ?? 1;

            if (code.HasValue && !hasSeenCodeInFile && name.Length > 0 && categoryId > 0)
                seenHeaderByCode[code.Value] = (name, categoryName, categoryId);

            Service? test = dbMatchByCode;
            test ??= existingByName.TryGetValue($"{categoryId}|{name}", out var byName) ? byName : null;

            if (!r.HasErrors)
                r.Status = test is not null ? RowStatus.Update : RowStatus.New;

            if (!commit || r.HasErrors) continue;

            var groupKey = code.HasValue ? $"ID:{code.Value}" : $"NAME:{categoryId}|{name}";
            if (!groupedTargets.TryGetValue(groupKey, out var target))
            {
                if (test is null)
                {
                    target = new Service { Name = name, CategoryID = categoryId };
                    db.Services.Add(target);
                }
                else
                {
                    target = test;
                    target.Name = name;
                    target.CategoryID = categoryId;
                }
                target.SaleRate = rate ?? target.SaleRate;
                target.DiagnosticType = diagnosticType;
                target.DeliveryHours = hours ?? target.DeliveryHours;
                groupedTargets[groupKey] = target;
            }

            if (paramName.Length > 0)
            {
                var unit = r.Optional("Unit");
                var range = r.Optional("ReferenceRange");
                pendingParams.Add((groupKey, paramName, unit, range, sortOrder));
            }
        }

        if (!commit || pendingParams.Count == 0) return;

        await db.SaveChangesAsync();
        var serviceIds = groupedTargets.Values.Select(s => s.ID).Where(id => id > 0).ToHashSet();
        var existingParams = await db.LabTestParameters.Where(p => serviceIds.Contains(p.ServiceID)).ToListAsync();

        foreach (var (key, paramName, unit, range, sortOrder) in pendingParams)
        {
            if (!groupedTargets.TryGetValue(key, out var svc) || svc.ID <= 0) continue;
            var existingParam = existingParams.FirstOrDefault(p =>
                p.ServiceID == svc.ID && string.Equals(p.ParameterName.Trim(), paramName, StringComparison.OrdinalIgnoreCase));
            if (existingParam is null)
            {
                var added = new LabTestParameter
                {
                    ServiceID = svc.ID,
                    ParameterName = paramName,
                    Unit = unit,
                    ReferenceRange = range,
                    SortOrder = sortOrder
                };
                db.LabTestParameters.Add(added);
                existingParams.Add(added);
            }
            else
            {
                existingParam.Unit = unit;
                existingParam.ReferenceRange = range;
                existingParam.SortOrder = sortOrder;
            }
        }
    }

    // ─────────────────────────── Types ───────────────────────────

    private enum ActiveModalKind { None, Template, Export, Import }

    private sealed record DependentInfo(string Name, int Count, string? SetupUrl, string? SetupLabel);

    private sealed record ColumnDef(string Name, bool Required, string Hint, string Example);

    private sealed record UploadType(string Key, string Title, string TitleBn, string Icon, string? Module,
        string Description, string DescriptionBn, ColumnDef[] Columns);

    private enum RowStatus { New, Update, Skip, Error }

    private sealed class ImportRow(int rowNo, Dictionary<string, string> values)
    {
        public int RowNo { get; } = rowNo;
        public string Label { get; set; } = string.Empty;
        public RowStatus Status { get; set; } = RowStatus.New;
        public List<string> Messages { get; } = [];
        public List<(string Column, string BadValue, IReadOnlyList<string> Options)> FixableLookups { get; } = [];
        public bool HasErrors => Status == RowStatus.Error;

        public void Reset() { Status = RowStatus.New; Messages.Clear(); FixableLookups.Clear(); Label = string.Empty; }

        public void Error(string message) { Status = RowStatus.Error; Messages.Add(message); }

        public string Get(string column) => values.TryGetValue(Normalize(column), out var v) ? v.Trim() : string.Empty;

        public void Set(string column, string value) => values[Normalize(column)] = value;

        public string? Optional(string column) => Get(column) is { Length: > 0 } v ? v : null;

        public string Required(string column, int maxLength)
        {
            var v = Get(column);
            if (v.Length == 0) Error($"{column} is required");
            else if (v.Length > maxLength) Error($"{column} is longer than {maxLength} characters");
            return v;
        }

        public void MaxLen(string column, int maxLength)
        {
            if (Get(column).Length > maxLength) Error($"{column} is longer than {maxLength} characters");
        }

        public decimal? Decimal(string column, bool required = false)
        {
            var v = Get(column);
            if (v.Length == 0)
            {
                if (required) Error($"{column} is required");
                return null;
            }
            if (!TabularFileReader.TryParseDecimal(v, out var d) || d < 0) { Error($"{column} must be a non-negative number"); return null; }
            return d;
        }

        public int? Int(string column)
        {
            var d = Decimal(column);
            if (d is null) return null;
            if (d != Math.Floor(d.Value) || d > int.MaxValue) { Error($"{column} must be a whole number"); return null; }
            return (int)d.Value;
        }

        public DateTime? Date(string column)
        {
            var v = Get(column);
            if (v.Length == 0) return null;
            if (TabularFileReader.TryParseDate(v, out var d)) return d;
            Error($"{column} is not a valid date (use yyyy-mm-dd)");
            return null;
        }

        public bool? Bool(string column)
        {
            var v = Get(column);
            if (v.Length == 0) return null;
            var b = TabularFileReader.ParseBool(v);
            if (b is null) Error($"{column} must be Yes or No");
            return b;
        }
    }
}


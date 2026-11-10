namespace HPM.Web.Security;

public sealed record LicensedModule(string Code, string Name, string[] Dependencies, string[] PageNames);

public static class LicensedModules
{
    public const string Registration = "registration";
    public const string Appointments = "appointments";
    public const string Diagnostics = "diagnostics";
    public const string Doctor = "doctor";
    public const string Pharmacy = "pharmacy";
    public const string Ipd = "ipd";
    public const string Emergency = "emergency";
    public const string Ot = "ot";
    public const string Nursing = "nursing";
    public const string BloodBank = "blood-bank";
    public const string Canteen = "canteen";
    public const string Accounts = "accounts";
    public const string Hrm = "hrm";
    public const string Store = "store";
    public const string Reports = "reports";
    public const string Mis = "mis";
    public const string PatientPortal = "patient-portal";

    public static readonly IReadOnlyList<LicensedModule> All =
    [
        new(Registration, "Patient Registration", [], ["Patients"]),
        new(Appointments, "Doctor Appointments", [Registration], ["Appointments"]),
        new(Diagnostics, "Diagnostics & Lab", [Registration], ["DiagnosticBilling", "DiagnosticCashMemoReturns", "DiagnosticResults", "DiagnosticWorklist", "DiagnosticSampleCollection", "DiagnosticImagingWorklist", "DiagnosticMasters", "DiagnosticTestMasters", "PublicLabReport"]),
        new(Doctor, "Doctor & Prescriptions", [Registration], ["DoctorDesk", "DoctorQueue", "DoctorFollowUps", "DoctorMyEarnings", "PharmacyPrescriptions", "DiagnosticOrders", "PatientDocuments", "MedicalCertificates", "DoctorCommissionMaster"]),
        new(Pharmacy, "Pharmacy", [Registration], ["PharmacyMasters", "PharmacyPos", "PharmacyRequisition", "PharmacyPurchases", "PharmacyStockControl", "PharmacyStock", "PharmacyReturnsPayments", "PharmacySalesReturns"]),
        new(Ipd, "IPD & Hospital", [Registration], ["IpdAdmissions", "IpdBedOccupancy", "IpdBedTurnover", "IpdPatientServices", "IpdMoneyReceipts", "IpdMedicalDischargeOrders", "IpdDischargeBilling", "IpdMasters"]),
        new(Emergency, "Emergency", [Registration], ["EmergencyServices", "Tickets"]),
        new(Ot, "Operation Theatre", [Registration], ["OperationSchedule", "OperationTheatreNotes", "OtCashMemos"]),
        new(Nursing, "Nursing", [Ipd], ["NurseDesk"]),
        new(BloodBank, "Blood Bank & Transfusion", [Registration], ["BloodBank"]),
        new(Canteen, "Canteen", [], ["CanteenSales", "CanteenProducts", "CanteenChallanPage"]),
        new(Accounts, "Accounts", [], ["Accounts", "FinancialReports"]),
        new(Hrm, "HRM, Rostering & Payroll", [], ["Hrm"]),
        new(Store, "Supply Chain & Store", [], ["StoreProductsPage", "StoreChallanPage"]),
        new(Reports, "Operational Reports", [], ["Reports"]),
        new(Mis, "MIS Reports", [Reports], ["MisReports"]),
        new(PatientPortal, "Patient Portal", [Registration], ["PatientPortal", "PatientPortalLogin", "PatientPortalDocument", "PatientPortalQueries"])
    ];

    public static readonly IReadOnlyDictionary<string, LicensedModule> ByCode = All.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);
    private static readonly IReadOnlyDictionary<string, string> ByPage = All.SelectMany(x => x.PageNames.Select(name => (name, x.Code)))
        .ToDictionary(x => x.name, x => x.Code, StringComparer.OrdinalIgnoreCase);

    public static string? ForPage(Type pageType) => ByPage.GetValueOrDefault(pageType.Name);

    public static string? ForPath(string? href)
    {
        var path = (href ?? "").Split('?')[0].TrimEnd('/').ToLowerInvariant();
        if (path.StartsWith("/patient") && !path.StartsWith("/patients")) return PatientPortal;
        if (path.StartsWith("/portal/report")) return Diagnostics;
        if (path.StartsWith("/diagnostic/")) return Diagnostics;
        if (path.StartsWith("/ipd/") || path == "/masters/ipd") return Ipd;
        if (path.StartsWith("/pharmacy/")) return Pharmacy;
        if (path.StartsWith("/canteen")) return Canteen;
        if (path.StartsWith("/store/")) return Store;
        if (path.StartsWith("/reports/mis")) return Mis;
        if (path.StartsWith("/reports/financial") || path == "/accounts") return Accounts;
        if (path == "/reports") return Reports;
        if (path == "/blood-bank") return BloodBank;
        if (path == "/admin/patient-queries") return PatientPortal;
        if (path == "/appointments") return Appointments;
        if (path == "/patients") return Registration;
        if (path.StartsWith("/masters/doctor-commission")) return Doctor;
        if (path.StartsWith("/patients/documents") || path.StartsWith("/patient-documents")) return Doctor;
        if (path == "/doctor" || path.StartsWith("/doctor/")) return Doctor;
        if (path == "/medical/nurse-desk" || path == "/medical/stickers") return Nursing;
        if (path.StartsWith("/medical/certificates")) return Doctor;
        if (path.StartsWith("/medical/ot-") || path.StartsWith("/medical/operation") || path == "/ot-cash-memos") return Ot;
        if (path == "/emergency-services" || path == "/tickets") return Emergency;
        if (path == "/hrm") return Hrm;
        return null;
    }

    public static HashSet<string> WithDependencies(IEnumerable<string> requested)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { Registration };
        foreach (var code in requested)
            Add(code, result);
        return result;
    }

    private static void Add(string code, HashSet<string> result)
    {
        if (!ByCode.TryGetValue(code, out var module) || !result.Add(code)) return;
        foreach (var dependency in module.Dependencies) Add(dependency, result);
    }
}

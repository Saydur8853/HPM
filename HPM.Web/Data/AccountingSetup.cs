using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;
using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

/// <summary>Stable account codes. Account IDs differ per database, so postings resolve them by code.</summary>
public static class AccountCodes
{
    // Assets
    public const string Cash = "1000";
    public const string Bank = "1010";
    public const string Mfs = "1020";
    public const string PatientReceivable = "1100";
    public const string CanteenReceivable = "1110";
    public const string MedicineInventory = "1200";
    public const string MedicalEquipment = "1500";
    public const string Furniture = "1510";
    public const string Building = "1520";
    public const string ComputerIt = "1530";
    public const string Vehicle = "1540";
    public const string Electrical = "1550";
    // Accumulated depreciation (contra assets): 15x0 -> 16x0
    public const string AccDepMedicalEquipment = "1600";
    public const string AccDepFurniture = "1610";
    public const string AccDepBuilding = "1620";
    public const string AccDepComputerIt = "1630";
    public const string AccDepVehicle = "1640";
    public const string AccDepElectrical = "1650";
    // Liabilities
    public const string SupplierPayable = "2000";
    public const string StoreSupplierPayable = "2010";
    public const string CanteenSupplierPayable = "2020";
    public const string PatientAdvance = "2100";
    public const string DoctorPayable = "2200";
    public const string SalaryPayable = "2300";
    // Equity
    public const string Capital = "3000";
    public const string RetainedEarnings = "3200";
    // Income
    public const string HospitalRevenue = "4000";
    public const string PharmacySales = "4010";
    public const string OpdIncome = "4100";
    public const string DiagnosticIncome = "4110";
    public const string EmergencyIncome = "4120";
    public const string OtIncome = "4130";
    public const string IpdBedIncome = "4140";
    public const string IpdServiceIncome = "4145";
    public const string BloodBankIncome = "4150";
    public const string CanteenSales = "4160";
    public const string OtherIncome = "4900";
    // Expenses
    public const string MedicinePurchase = "5000";
    public const string CostOfMedicineSold = "5005";
    public const string LabSupplies = "5010";
    public const string CanteenPurchase = "5020";
    public const string StorePurchase = "5030";
    public const string GeneralExpense = "5100";
    public const string DoctorSalary = "5200";
    public const string DoctorCommission = "5220";
    public const string StaffSalary = "5210";
    public const string Electricity = "5300";
    public const string Gas = "5310";
    public const string Water = "5320";
    public const string Telephone = "5330";
    public const string Cleaning = "5400";
    public const string Repair = "5500";
    public const string Rent = "5600";
    public const string DiscountAllowed = "5700";
    public const string Depreciation = "5800";
    public const string Misc = "5900";
}

public sealed record ChartSeed(string Code, string Name, string Type, string Group);

public static class AccountingSetup
{
    public const string DoctorVisitShareKey = "DOCTOR_VISIT_SHARE";
    public const string BloodBagRateKey = "BLOOD_BAG_RATE";

    /// <summary>The standard hospital chart. Group drives the Profit &amp; Loss layout.</summary>
    public static IReadOnlyList<ChartSeed> Chart() =>
    [
        new(AccountCodes.Cash, "Cash in Hand", "Asset", "Cash & Bank"),
        new(AccountCodes.Bank, "Bank Account", "Asset", "Cash & Bank"),
        new(AccountCodes.Mfs, "Mobile Banking (MFS)", "Asset", "Cash & Bank"),
        new(AccountCodes.PatientReceivable, "Patient Receivable", "Asset", "Receivables"),
        new(AccountCodes.CanteenReceivable, "Canteen Staff Receivable", "Asset", "Receivables"),
        new(AccountCodes.MedicineInventory, "Medicine Inventory", "Asset", "Inventory"),
        new(AccountCodes.MedicalEquipment, "Medical Equipment & Machines", "Asset", "Fixed Assets"),
        new(AccountCodes.Furniture, "Furniture & Fixtures", "Asset", "Fixed Assets"),
        new(AccountCodes.Building, "Land & Building", "Asset", "Fixed Assets"),
        new(AccountCodes.ComputerIt, "Computer & IT Equipment", "Asset", "Fixed Assets"),
        new(AccountCodes.Vehicle, "Vehicle & Ambulance", "Asset", "Fixed Assets"),
        new(AccountCodes.Electrical, "Electrical & Generator", "Asset", "Fixed Assets"),
        new(AccountCodes.AccDepMedicalEquipment, "Acc. Depreciation - Medical Equipment", "Asset", "Accumulated Depreciation"),
        new(AccountCodes.AccDepFurniture, "Acc. Depreciation - Furniture", "Asset", "Accumulated Depreciation"),
        new(AccountCodes.AccDepBuilding, "Acc. Depreciation - Building", "Asset", "Accumulated Depreciation"),
        new(AccountCodes.AccDepComputerIt, "Acc. Depreciation - Computer & IT", "Asset", "Accumulated Depreciation"),
        new(AccountCodes.AccDepVehicle, "Acc. Depreciation - Vehicle", "Asset", "Accumulated Depreciation"),
        new(AccountCodes.AccDepElectrical, "Acc. Depreciation - Electrical", "Asset", "Accumulated Depreciation"),
        new(AccountCodes.SupplierPayable, "Supplier Payable", "Liability", "Payables"),
        new(AccountCodes.StoreSupplierPayable, "Store Supplier Payable", "Liability", "Payables"),
        new(AccountCodes.CanteenSupplierPayable, "Canteen Supplier Payable", "Liability", "Payables"),
        new(AccountCodes.PatientAdvance, "Patient Advance", "Liability", "Payables"),
        new(AccountCodes.DoctorPayable, "Doctor Fee & Commission Payable", "Liability", "Payables"),
        new(AccountCodes.SalaryPayable, "Salary Payable", "Liability", "Payables"),
        new(AccountCodes.Capital, "Owner's Capital", "Equity", "Equity"),
        new(AccountCodes.RetainedEarnings, "Retained Earnings", "Equity", "Equity"),
        new(AccountCodes.HospitalRevenue, "Hospital Service Revenue", "Income", "Other Services"),
        new(AccountCodes.PharmacySales, "Pharmacy Sales", "Income", "Pharmacy"),
        new(AccountCodes.OpdIncome, "OPD Consultation Income", "Income", "OPD"),
        new(AccountCodes.DiagnosticIncome, "Diagnostic & Lab Income", "Income", "Diagnostics & Lab"),
        new(AccountCodes.EmergencyIncome, "Emergency Income", "Income", "Emergency"),
        new(AccountCodes.OtIncome, "OT Income", "Income", "OT"),
        new(AccountCodes.IpdBedIncome, "IPD Bed & Cabin Income", "Income", "IPD"),
        new(AccountCodes.IpdServiceIncome, "IPD Service Income", "Income", "IPD"),
        new(AccountCodes.BloodBankIncome, "Blood Bank Income", "Income", "Blood Bank"),
        new(AccountCodes.CanteenSales, "Canteen Sales", "Income", "Canteen"),
        new(AccountCodes.OtherIncome, "Other Income", "Income", "Other Services"),
        new(AccountCodes.MedicinePurchase, "Medicine Purchase", "Expense", "Pharmacy"),
        new(AccountCodes.CostOfMedicineSold, "Cost of Medicine Sold", "Expense", "Pharmacy"),
        new(AccountCodes.LabSupplies, "Lab Reagents & Diagnostic Supplies", "Expense", "Diagnostics & Lab"),
        new(AccountCodes.CanteenPurchase, "Canteen Food & Raw Materials", "Expense", "Canteen"),
        new(AccountCodes.StorePurchase, "Medical & General Consumables", "Expense", "Operations"),
        new(AccountCodes.GeneralExpense, "General Expense", "Expense", "Operations"),
        new(AccountCodes.DoctorSalary, "Doctor Salary & Fees", "Expense", "Salaries"),
        new(AccountCodes.StaffSalary, "Staff Salary & Wages", "Expense", "Salaries"),
        new(AccountCodes.DoctorCommission, "Doctor Referral Commission", "Expense", "Salaries"),
        new(AccountCodes.Electricity, "Electricity Bill", "Expense", "Utilities"),
        new(AccountCodes.Gas, "Gas Bill", "Expense", "Utilities"),
        new(AccountCodes.Water, "Water Bill", "Expense", "Utilities"),
        new(AccountCodes.Telephone, "Internet & Telephone", "Expense", "Utilities"),
        new(AccountCodes.Cleaning, "Cleaning & Hygiene Supplies", "Expense", "Operations"),
        new(AccountCodes.Repair, "Repair & Maintenance", "Expense", "Operations"),
        new(AccountCodes.Rent, "Rent", "Expense", "Operations"),
        new(AccountCodes.DiscountAllowed, "Discount Allowed", "Expense", "Discounts"),
        new(AccountCodes.Depreciation, "Depreciation (Equipment & Furniture)", "Expense", "Operations"),
        new(AccountCodes.Misc, "Miscellaneous Expense", "Expense", "Operations"),
    ];

    public static string GroupOf(AccountHead head)
    {
        var seed = Chart().FirstOrDefault(x => x.Code == head.Code);
        if (seed is not null) return seed.Group;
        return head.AccountType switch { "Income" => "Other Services", "Expense" => "Operations", _ => head.AccountType };
    }

    // ---------------------------------------------------------------- startup
    public static async Task EnsureAsync(HpmDbContext db)
    {
        const string schemaSql = """
            IF OBJECT_ID(N'[Accounts_Setting]', N'U') IS NULL
                CREATE TABLE [Accounts_Setting] (
                    [Setting_Key] nvarchar(80) NOT NULL CONSTRAINT [PK_Accounts_Setting] PRIMARY KEY,
                    [Setting_Value] nvarchar(400) NULL,
                    [Updated_At] datetime NOT NULL CONSTRAINT [DF_Accounts_Setting_Updated_At] DEFAULT (GETDATE()));
            IF OBJECT_ID(N'[Blood_Issue]', N'U') IS NOT NULL AND COL_LENGTH(N'Blood_Issue', N'ChargeAmount') IS NULL
                ALTER TABLE [Blood_Issue] ADD [ChargeAmount] decimal(18,2) NOT NULL CONSTRAINT [DF_Blood_Issue_ChargeAmount] DEFAULT (0);
            IF OBJECT_ID(N'[Blood_Issue]', N'U') IS NOT NULL AND COL_LENGTH(N'Blood_Issue', N'PaymentMethod') IS NULL
                ALTER TABLE [Blood_Issue] ADD [PaymentMethod] nvarchar(30) NULL;
            IF OBJECT_ID(N'[Blood_Issue]', N'U') IS NOT NULL AND COL_LENGTH(N'Blood_Issue', N'PaymentRef') IS NULL
                ALTER TABLE [Blood_Issue] ADD [PaymentRef] nvarchar(60) NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(schemaSql);
        await db.Database.ExecuteSqlRawAsync(CostCenterSchemaSql);
        await db.Database.ExecuteSqlRawAsync(CostCenterSeedSql);

        var existing = (await db.AccountHeads.AsNoTracking().Select(x => x.Code).ToListAsync()).ToHashSet();
        foreach (var seed in Chart().Where(x => !existing.Contains(x.Code)))
            db.AccountHeads.Add(new AccountHead { Code = seed.Code, Name = seed.Name, AccountType = seed.Type, IsActive = true });
        await db.SaveChangesAsync();

        // One-time reclassification: older vouchers credited every service to the single
        // "Hospital Service Revenue" head. Move them to the department head using the voucher
        // reference (or the reference of the voucher a reversal undoes). Income stays income,
        // so totals and profit do not change; only the breakdown becomes visible.
        const string reclassSql = """
            DECLARE @rev int = (SELECT [ID] FROM [Accounts_Chart] WHERE [Code] = N'4000');
            DECLARE @opd int = (SELECT [ID] FROM [Accounts_Chart] WHERE [Code] = N'4100');
            DECLARE @diag int = (SELECT [ID] FROM [Accounts_Chart] WHERE [Code] = N'4110');
            DECLARE @emg int = (SELECT [ID] FROM [Accounts_Chart] WHERE [Code] = N'4120');
            DECLARE @ot int = (SELECT [ID] FROM [Accounts_Chart] WHERE [Code] = N'4130');
            DECLARE @ipd int = (SELECT [ID] FROM [Accounts_Chart] WHERE [Code] = N'4145');
            IF @rev IS NOT NULL AND @opd IS NOT NULL AND @diag IS NOT NULL AND @emg IS NOT NULL AND @ot IS NOT NULL AND @ipd IS NOT NULL
            BEGIN
                ;WITH src AS (
                    SELECT v.[ID], COALESCE(o.[Reference], v.[Reference]) AS [Ref]
                    FROM [Accounts_Voucher] v
                    LEFT JOIN [Accounts_Voucher] o ON o.[ID] = v.[Reversal_Of_Voucher_ID])
                UPDATE l SET [Account_ID] = CASE
                        WHEN s.[Ref] LIKE N'OPD-TICKET:%' THEN @opd
                        WHEN s.[Ref] LIKE N'DIAGNOSTIC%' THEN @diag
                        WHEN s.[Ref] LIKE N'EMERGENCY:%' THEN @emg
                        WHEN s.[Ref] LIKE N'OT-MEMO:%' THEN @ot
                        ELSE @ipd END
                FROM [Accounts_Voucher_Line] l
                JOIN src s ON s.[ID] = l.[Voucher_ID]
                WHERE l.[Account_ID] = @rev
                  AND (s.[Ref] LIKE N'OPD-TICKET:%' OR s.[Ref] LIKE N'DIAGNOSTIC%' OR s.[Ref] LIKE N'EMERGENCY:%'
                       OR s.[Ref] LIKE N'OT-MEMO:%' OR s.[Ref] LIKE N'IPD-FINAL-BILL:%');
            END
            """;
        await db.Database.ExecuteSqlRawAsync(reclassSql);
        IdCache.Clear();
        InvalidateCenters();

        // One-time: give every old income/expense line its department, from the head it was posted to.
        await LoadCenterMapAsync(db);
        var map = centerByHead!;
        foreach (var g in map.Where(x => x.Value.HasValue).GroupBy(x => x.Value!.Value))
        {
            var headIds = string.Join(",", g.Select(x => x.Key));
            await db.Database.ExecuteSqlRawAsync(
                $"UPDATE [Accounts_Voucher_Line] SET [Cost_Center_ID] = {g.Key} WHERE [Cost_Center_ID] IS NULL AND [Account_ID] IN ({headIds})");
        }
    }

    // ---------------------------------------------------------------- cost centers (departments)
    // Ideas and table layout taken from the cost-center accounting branch; account codes follow this chart.
    private const string CostCenterSchemaSql = """
        IF COL_LENGTH(N'Accounts_Voucher_Line', N'Cost_Center_ID') IS NULL
        BEGIN
            ALTER TABLE [Accounts_Voucher_Line] ADD [Cost_Center_ID] int NULL;
            EXEC(N'CREATE INDEX [IX_Accounts_Voucher_Line_Cost_Center] ON [Accounts_Voucher_Line] ([Cost_Center_ID], [Account_ID])');
        END
        IF OBJECT_ID(N'[Service_Category]', N'U') IS NOT NULL AND COL_LENGTH(N'Service_Category', N'Cost_Center_ID') IS NULL
            ALTER TABLE [Service_Category] ADD [Cost_Center_ID] int NULL;
        IF OBJECT_ID(N'[Bed_Category]', N'U') IS NOT NULL AND COL_LENGTH(N'Bed_Category', N'Cost_Center_ID') IS NULL
            ALTER TABLE [Bed_Category] ADD [Cost_Center_ID] int NULL;

        IF OBJECT_ID(N'[Accounts_Cost_Center]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Cost_Center] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Cost_Center] PRIMARY KEY,
                [Code] nvarchar(20) NOT NULL,
                [Name] nvarchar(100) NOT NULL,
                [Center_Type] nvarchar(20) NOT NULL,
                [Incharge] nvarchar(100) NULL,
                [Area_Sq_Ft] numeric(18,2) NULL,
                [Is_Active] bit NOT NULL CONSTRAINT [DF_Accounts_Cost_Center_Active] DEFAULT 1,
                [Is_System] bit NOT NULL CONSTRAINT [DF_Accounts_Cost_Center_System] DEFAULT 0);
            CREATE UNIQUE INDEX [IX_Accounts_Cost_Center_Code] ON [Accounts_Cost_Center] ([Code]);
        END

        IF OBJECT_ID(N'[Accounts_Fixed_Asset]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Fixed_Asset] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Fixed_Asset] PRIMARY KEY,
                [Asset_Code] nvarchar(30) NOT NULL,
                [Name] nvarchar(150) NOT NULL,
                [Serial_No] nvarchar(100) NULL,
                [Asset_Account_ID] int NOT NULL,
                [Cost_Center_ID] int NOT NULL,
                [Location] nvarchar(150) NULL,
                [Supplier] nvarchar(150) NULL,
                [Invoice_No] nvarchar(50) NULL,
                [Purchase_Date] date NOT NULL,
                [In_Service_Date] date NOT NULL,
                [Cost] numeric(18,2) NOT NULL,
                [Salvage_Value] numeric(18,2) NOT NULL,
                [Useful_Life_Years] int NOT NULL,
                [Opening_Acc_Depreciation] numeric(18,2) NOT NULL,
                [Status] nvarchar(20) NOT NULL,
                [Warranty_End] date NULL,
                [Disposed_Date] date NULL,
                [Disposal_Amount] numeric(18,2) NULL,
                [Purchase_Voucher_ID] bigint NULL,
                [Disposal_Voucher_ID] bigint NULL,
                [Entered_By] int NOT NULL,
                [Trans_Date] datetime NOT NULL);
            CREATE UNIQUE INDEX [IX_Accounts_Fixed_Asset_Code] ON [Accounts_Fixed_Asset] ([Asset_Code]);
        END

        IF OBJECT_ID(N'[Accounts_Fixed_Asset_Transfer]', N'U') IS NULL
            CREATE TABLE [Accounts_Fixed_Asset_Transfer] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Fixed_Asset_Transfer] PRIMARY KEY,
                [Asset_ID] int NOT NULL,
                [From_Cost_Center_ID] int NOT NULL,
                [To_Cost_Center_ID] int NOT NULL,
                [Transfer_Date] date NOT NULL,
                [Reason] nvarchar(300) NULL,
                [Entered_By] int NOT NULL);

        IF OBJECT_ID(N'[Accounts_Fixed_Asset_Depreciation]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Fixed_Asset_Depreciation] (
                [ID] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Fixed_Asset_Depreciation] PRIMARY KEY,
                [Asset_ID] int NOT NULL,
                [Period_Month] date NOT NULL,
                [Amount] numeric(18,2) NOT NULL,
                [Cost_Center_ID] int NOT NULL,
                [Voucher_ID] bigint NOT NULL);
            CREATE UNIQUE INDEX [IX_Accounts_FA_Depreciation_Asset_Month] ON [Accounts_Fixed_Asset_Depreciation] ([Asset_ID], [Period_Month]);
        END

        IF OBJECT_ID(N'[Accounts_Allocation_Rule]', N'U') IS NULL
            CREATE TABLE [Accounts_Allocation_Rule] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Allocation_Rule] PRIMARY KEY,
                [Source_Cost_Center_ID] int NOT NULL,
                [Basis] nvarchar(20) NOT NULL,
                [Target_Cost_Center_ID] int NULL,
                [Weight] numeric(18,4) NULL,
                [Is_Active] bit NOT NULL CONSTRAINT [DF_Accounts_Allocation_Rule_Active] DEFAULT 1);

        IF OBJECT_ID(N'[Accounts_Allocation_Run]', N'U') IS NULL
        BEGIN
            CREATE TABLE [Accounts_Allocation_Run] (
                [ID] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts_Allocation_Run] PRIMARY KEY,
                [Period_Month] date NOT NULL,
                [Voucher_ID] bigint NOT NULL,
                [Run_By] int NOT NULL,
                [Run_At] datetime NOT NULL);
            CREATE UNIQUE INDEX [IX_Accounts_Allocation_Run_Month] ON [Accounts_Allocation_Run] ([Period_Month]);
        END
        """;

    // Standard departments; shared (service/admin) costs are spread over revenue departments by income until changed.
    private const string CostCenterSeedSql = """
        DECLARE @cc TABLE ([Code] nvarchar(20), [Name] nvarchar(100), [Type] nvarchar(20));
        INSERT INTO @cc VALUES
            (N'CC-101', N'OPD (Outpatient)', N'Revenue'),
            (N'CC-102', N'Emergency', N'Revenue'),
            (N'CC-103', N'IPD Ward & Cabin', N'Revenue'),
            (N'CC-104', N'ICU / CCU / NICU', N'Revenue'),
            (N'CC-105', N'Operation Theatre (OT)', N'Revenue'),
            (N'CC-106', N'Pathology (Laboratory)', N'Revenue'),
            (N'CC-107', N'Radiology & Imaging', N'Revenue'),
            (N'CC-108', N'Cardiology & Procedures', N'Revenue'),
            (N'CC-109', N'Pharmacy', N'Revenue'),
            (N'CC-110', N'Blood Bank', N'Revenue'),
            (N'CC-111', N'Ambulance', N'Revenue'),
            (N'CC-112', N'Canteen', N'Revenue'),
            (N'CC-201', N'Reception & Billing', N'Service'),
            (N'CC-202', N'Nursing Administration', N'Service'),
            (N'CC-203', N'Housekeeping & Laundry', N'Service'),
            (N'CC-204', N'Maintenance & Biomedical', N'Service'),
            (N'CC-205', N'Store & Procurement', N'Service'),
            (N'CC-206', N'Kitchen (Patient Food)', N'Service'),
            (N'CC-301', N'Administration & Management', N'Admin'),
            (N'CC-302', N'Accounts & Finance', N'Admin'),
            (N'CC-303', N'HR', N'Admin'),
            (N'CC-304', N'IT', N'Admin'),
            (N'CC-399', N'General (Shared Cost)', N'Admin');
        INSERT INTO [Accounts_Cost_Center] ([Code], [Name], [Center_Type], [Is_Active], [Is_System])
        SELECT c.[Code], c.[Name], c.[Type], 1, 1 FROM @cc c
        WHERE NOT EXISTS (SELECT 1 FROM [Accounts_Cost_Center] x WHERE x.[Code] = c.[Code]);

        IF NOT EXISTS (SELECT 1 FROM [Accounts_Allocation_Rule])
            INSERT INTO [Accounts_Allocation_Rule] ([Source_Cost_Center_ID], [Basis], [Is_Active])
            SELECT [ID], N'Revenue', 1 FROM [Accounts_Cost_Center] WHERE [Center_Type] IN (N'Service', N'Admin');
        """;

    public static class Centers
    {
        public const string Opd = "CC-101", Emergency = "CC-102", Ward = "CC-103", Ot = "CC-105", Pathology = "CC-106",
            Imaging = "CC-107", Cardiology = "CC-108", Pharmacy = "CC-109", BloodBank = "CC-110", Ambulance = "CC-111",
            Canteen = "CC-112", Housekeeping = "CC-203", Maintenance = "CC-204", Store = "CC-205", General = "CC-399";
    }

    /// <summary>The department an income/expense head belongs to when the posting does not say. Null for balance-sheet heads.</summary>
    public static string? DefaultCenterCode(string headCode, string headType)
    {
        if (headType is not ("Income" or "Expense")) return null;
        return headCode switch
        {
            AccountCodes.OpdIncome or AccountCodes.DoctorSalary => Centers.Opd,
            AccountCodes.EmergencyIncome => Centers.Emergency,
            AccountCodes.IpdBedIncome or AccountCodes.IpdServiceIncome => Centers.Ward,
            AccountCodes.OtIncome => Centers.Ot,
            AccountCodes.DiagnosticIncome or AccountCodes.LabSupplies or AccountCodes.DoctorCommission => Centers.Pathology,
            AccountCodes.PharmacySales or AccountCodes.MedicinePurchase or AccountCodes.CostOfMedicineSold => Centers.Pharmacy,
            AccountCodes.BloodBankIncome => Centers.BloodBank,
            AccountCodes.CanteenSales or AccountCodes.CanteenPurchase => Centers.Canteen,
            AccountCodes.Cleaning => Centers.Housekeeping,
            AccountCodes.Repair => Centers.Maintenance,
            AccountCodes.StorePurchase => Centers.Store,
            _ => Centers.General
        };
    }

    private static Dictionary<int, int?>? centerByHead;
    private static Dictionary<string, int>? centerByCode;
    private static readonly SemaphoreSlim CenterGate = new(1, 1);

    public static void InvalidateCenters() { centerByHead = null; centerByCode = null; }

    private static async Task LoadCenterMapAsync(HpmDbContext db)
    {
        if (centerByHead is not null && centerByCode is not null) return;
        await CenterGate.WaitAsync();
        try
        {
            if (centerByHead is not null && centerByCode is not null) return;
            var codes = await db.CostCenters.AsNoTracking().Where(x => x.IsActive).ToDictionaryAsync(x => x.Code, x => x.ID, StringComparer.OrdinalIgnoreCase);
            var heads = await db.AccountHeads.AsNoTracking().Select(x => new { x.ID, x.Code, x.AccountType }).ToListAsync();
            centerByHead = heads.ToDictionary(h => h.ID, h => DefaultCenterCode(h.Code, h.AccountType) is { } c && codes.TryGetValue(c, out var id) ? id : (int?)null);
            centerByCode = codes;
        }
        finally { CenterGate.Release(); }
    }

    /// <summary>Active cost center ID by code, or null.</summary>
    public static async Task<int?> CenterIdAsync(HpmDbContext db, string code)
    {
        await LoadCenterMapAsync(db);
        return centerByCode!.TryGetValue(code, out var id) ? id : null;
    }

    /// <summary>Fills the department of new income/expense voucher lines that were posted without one. Called from SaveChangesAsync.</summary>
    public static async Task FillMissingCentersAsync(HpmDbContext db, IEnumerable<AccountVoucherLine> lines)
    {
        var missing = lines.Where(x => x.CostCenterID is null).ToList();
        if (missing.Count == 0) return;
        await LoadCenterMapAsync(db);
        foreach (var l in missing)
        {
            if (!centerByHead!.TryGetValue(l.AccountID, out var cc))
            {
                InvalidateCenters(); // a head added after the map was built
                await LoadCenterMapAsync(db);
                centerByHead!.TryGetValue(l.AccountID, out cc);
            }
            l.CostCenterID = cc;
        }
    }

    /// <summary>Income department of each diagnostic test: its category's cost center, else Pathology / Imaging / Cardiology by test type.</summary>
    public static async Task<Dictionary<int, int?>> DiagnosticCentersAsync(HpmDbContext db, IEnumerable<int> serviceIds)
    {
        var ids = serviceIds.Distinct().ToArray();
        var services = await db.Services.AsNoTracking().Where(x => ids.Contains(x.ID)).ToListAsync();
        var catIds = services.Where(x => x.CategoryID.HasValue).Select(x => x.CategoryID!.Value).Distinct().ToArray();
        var categories = await db.ServiceCategories.AsNoTracking().Where(x => catIds.Contains(x.ID)).ToDictionaryAsync(x => x.ID);
        var result = new Dictionary<int, int?>();
        foreach (var id in ids)
        {
            var service = services.FirstOrDefault(x => x.ID == id);
            var category = service?.CategoryID is int c && categories.TryGetValue(c, out var cat) ? cat : null;
            if (category?.CostCenterID is int own) { result[id] = own; continue; }
            var code = service is null ? Centers.Pathology
                : DiagnosticTestRouting.IsImaging(service, category) ? Centers.Imaging
                : DiagnosticTestRouting.IsProcedure(service, category) ? Centers.Cardiology
                : Centers.Pathology;
            result[id] = await CenterIdAsync(db, code);
        }
        return result;
    }

    /// <summary>Department of a bed: its category's cost center, else the general ward.</summary>
    public static async Task<int?> BedCenterAsync(HpmDbContext db, int? bedCabinId)
    {
        if (bedCabinId is int bedId)
        {
            var cc = await (from b in db.BedCabins join c in db.BedCategories on b.CategoryID equals c.ID where b.ID == bedId select c.CostCenterID).FirstOrDefaultAsync();
            if (cc.HasValue) return cc;
        }
        return await CenterIdAsync(db, Centers.Ward);
    }

    /// <summary>Accumulated-depreciation head paired with an asset head: 15x0 -> 16x0.</summary>
    public static string AccumulatedDepreciationCode(string assetCode) =>
        assetCode.Length == 4 && assetCode.StartsWith("15", StringComparison.Ordinal) ? "16" + assetCode[2..] : AccountCodes.AccDepMedicalEquipment;

    // ---------------------------------------------------------------- account lookup
    private static readonly ConcurrentDictionary<string, int> IdCache = new();

    public static async Task<int> IdAsync(HpmDbContext db, string code)
    {
        if (IdCache.TryGetValue(code, out var cached)) return cached;
        var id = await db.AccountHeads.AsNoTracking().Where(x => x.Code == code).Select(x => (int?)x.ID).FirstOrDefaultAsync()
                 ?? throw new InvalidOperationException($"Account head {code} is missing from the chart of accounts. Restart the application to re-create it.");
        IdCache[code] = id;
        return id;
    }

    /// <summary>Cash / Bank / MFS ledger for a payment method name. An Accounts_Setting row "PAYMENT_ACCOUNT:{name}" overrides the guess.</summary>
    public static async Task<int> PaymentAccountIdAsync(HpmDbContext db, string? method)
    {
        var name = (method ?? "Cash").Trim();
        var mapped = await SettingAsync(db, $"PAYMENT_ACCOUNT:{name}");
        if (!string.IsNullOrWhiteSpace(mapped)) return await IdAsync(db, mapped.Trim());
        return await IdAsync(db, GuessPaymentCode(name));
    }

    public static string GuessPaymentCode(string? method)
    {
        var n = (method ?? "").Trim().ToLowerInvariant();
        if (n.Length == 0 || n == "cash") return AccountCodes.Cash;
        string[] mfs = ["mfs", "bkash", "nagad", "rocket", "upay", "mobile", "tap", "surecash"];
        if (mfs.Any(n.Contains)) return AccountCodes.Mfs;
        string[] bank = ["bank", "card", "cheque", "check", "pos", "visa", "master", "amex", "transfer", "eft", "npsb"];
        if (bank.Any(n.Contains)) return AccountCodes.Bank;
        return AccountCodes.Cash;
    }

    public static bool IsCashMethod(string? method) => GuessPaymentCode(method) == AccountCodes.Cash;

    /// <summary>Store item category → expense (or asset) head.</summary>
    public static string StoreCategoryCode(string? category)
    {
        var n = (category ?? "").ToLowerInvariant();
        if (n.Contains("reagent") || n.Contains("lab") || n.Contains("kit") || n.Contains("cath")) return AccountCodes.LabSupplies;
        if (n.Contains("clean") || n.Contains("hygien") || n.Contains("housekeep") || n.Contains("linen") || n.Contains("laundry")) return AccountCodes.Cleaning;
        if (n.Contains("furniture")) return AccountCodes.Furniture;
        if (n.Contains("spare") || n.Contains("repair")) return AccountCodes.Repair;
        if (n.Contains("equipment") || n.Contains("machine") || n.Contains("instrument")) return AccountCodes.MedicalEquipment;
        if (n.Contains("stationery") || n.Contains("printing") || n.Contains("office")) return AccountCodes.GeneralExpense;
        return AccountCodes.StorePurchase;
    }

    // ---------------------------------------------------------------- settings
    public static async Task<string?> SettingAsync(HpmDbContext db, string key) =>
        await db.AccountSettings.AsNoTracking().Where(x => x.Key == key).Select(x => x.Value).FirstOrDefaultAsync();

    public static async Task<Dictionary<string, string?>> SettingsAsync(HpmDbContext db) =>
        await db.AccountSettings.AsNoTracking().ToDictionaryAsync(x => x.Key, x => x.Value);

    public static async Task SaveSettingAsync(HpmDbContext db, string key, string? value)
    {
        var row = await db.AccountSettings.FirstOrDefaultAsync(x => x.Key == key);
        if (string.IsNullOrWhiteSpace(value))
        {
            if (row is not null) db.AccountSettings.Remove(row);
            return;
        }
        if (row is null) db.AccountSettings.Add(new AccountSetting { Key = key, Value = value.Trim(), UpdatedAt = DateTime.Now });
        else { row.Value = value.Trim(); row.UpdatedAt = DateTime.Now; }
    }

    /// <summary>Doctor's share (percent) of an OPD visit fee. Per-doctor setting wins over the hospital default.</summary>
    public static async Task<decimal> DoctorVisitSharePercentAsync(HpmDbContext db, int doctorId)
    {
        var own = await SettingAsync(db, $"{DoctorVisitShareKey}:{doctorId}");
        var value = own ?? await SettingAsync(db, DoctorVisitShareKey);
        return decimal.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var pct)
            ? Math.Clamp(pct, 0, 100) : 0;
    }

    /// <summary>Accounts for an OPD ticket: hospital's income head, the doctor's share of the fee, and the doctor payable head.</summary>
    public static async Task<(int RevenueId, decimal DoctorShare, int DoctorPayableId)> OpdSplitAsync(HpmDbContext db, int doctorId, decimal fee)
    {
        var pct = await DoctorVisitSharePercentAsync(db, doctorId);
        return (await IdAsync(db, AccountCodes.OpdIncome), Math.Round(Math.Max(0, fee) * pct / 100m, 2), await IdAsync(db, AccountCodes.DoctorPayable));
    }

    // ---------------------------------------------------------------- doctor referral commission
    /// <summary>
    /// Referral commission for a diagnostic memo, using the same rules as the doctor's "My Earnings" page:
    /// rate per service category from Doctor_Commission_Settings (doctor row overrides the DoctorID = 0 default),
    /// falling back to 20% when the category has no rate.
    /// </summary>
    public static async Task<decimal> DiagnosticCommissionAsync(HpmDbContext db, int doctorId, IEnumerable<(int TestId, decimal Rate, decimal Quantity)> items)
    {
        var lines = items.ToList();
        if (doctorId <= 0 || lines.Count == 0) return 0;
        var rates = new Dictionary<int, decimal>();
        try
        {
            var conn = db.Database.GetDbConnection();
            var wasClosed = conn.State != System.Data.ConnectionState.Open;
            if (wasClosed) await conn.OpenAsync();
            try
            {
                await using var cmd = conn.CreateCommand();
                cmd.Transaction = db.Database.CurrentTransaction?.GetDbTransaction();
                cmd.CommandText = "SELECT CategoryID, CommissionPercent FROM Doctor_Commission_Settings WHERE DoctorID = @docId OR DoctorID = 0 ORDER BY DoctorID ASC";
                var p = cmd.CreateParameter();
                p.ParameterName = "@docId";
                p.Value = doctorId;
                cmd.Parameters.Add(p);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) rates[reader.GetInt32(0)] = reader.GetDecimal(1);
            }
            finally { if (wasClosed) await conn.CloseAsync(); }
        }
        catch
        {
            // table not created yet: every category uses the default rate
        }

        var testIds = lines.Select(x => x.TestId).Distinct().ToArray();
        var categoryByTest = await db.Services.AsNoTracking().Where(x => testIds.Contains(x.ID))
            .Select(x => new { x.ID, x.CategoryID }).ToDictionaryAsync(x => x.ID, x => x.CategoryID ?? 0);
        decimal total = 0;
        foreach (var l in lines)
        {
            var pct = rates.TryGetValue(categoryByTest.GetValueOrDefault(l.TestId), out var r) ? r : 20m;
            total += l.Rate * l.Quantity * pct / 100m;
        }
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Re-accrues a diagnostic memo's referral commission on what is still billed: memo quantities minus
    /// quantities on posted (not cancelled) returns. Call after the return rows are saved.
    /// </summary>
    public static async Task ResyncDiagnosticCommissionForMemoAsync(HpmDbContext db, long memoId, int userId)
    {
        var memo = await db.EmergencyMemos.AsNoTracking().Include(x => x.Items).FirstOrDefaultAsync(x => x.ID == memoId);
        if (memo is null || memo.ModuleID != 2) return;
        var returned = await (from ri in db.DiagnosticCashMemoReturnItems.AsNoTracking()
                              join r in db.DiagnosticCashMemoReturns.AsNoTracking() on ri.ReturnID equals r.ID
                              where r.MemoID == memoId && r.Status != "Cancelled"
                              select new { ri.CashMemoItemID, ri.Quantity })
            .ToListAsync();
        var returnedByItem = returned.GroupBy(x => x.CashMemoItemID).ToDictionary(g => g.Key, g => (decimal)g.Sum(x => x.Quantity));
        var items = memo.Items
            .Select(x => (x.TestID, (decimal)x.Rate, Math.Max(0, (decimal)x.Quantity - returnedByItem.GetValueOrDefault(x.ID))))
            .Where(x => x.Item3 > 0)
            .ToList();
        await SyncDiagnosticCommissionAsync(db, memo.ID, memo.DoctorID ?? 0, memo.TransDate, items, userId, memo.PatientName ?? "");
    }

    /// <summary>Accrue (or re-accrue after an edit) the referral commission of a diagnostic memo: Dr Commission expense, Cr Doctor payable.</summary>
    public static async Task SyncDiagnosticCommissionAsync(HpmDbContext db, long memoId, int doctorId, DateTime memoDate,
        IEnumerable<(int TestId, decimal Rate, decimal Quantity)> items, int userId, string patientName)
    {
        var amount = await DiagnosticCommissionAsync(db, doctorId, items);
        var expense = await IdAsync(db, AccountCodes.DoctorCommission);
        var payable = await IdAsync(db, AccountCodes.DoctorPayable);
        await OperationalAccounting.SyncJournalAsync(db, $"DOCTOR-COMMISSION:DIAGNOSTIC:{memoId}", memoDate, userId,
            $"Referral commission to doctor #{doctorId} on diagnostic memo #{memoId} ({patientName})",
            amount <= 0 ? [] : [(expense, amount, 0m), (payable, 0m, amount)]);
    }
}

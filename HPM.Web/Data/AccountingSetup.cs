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
    public const string MedicalEquipment = "1500";
    public const string Furniture = "1510";
    // Liabilities
    public const string SupplierPayable = "2000";
    public const string StoreSupplierPayable = "2010";
    public const string CanteenSupplierPayable = "2020";
    public const string PatientAdvance = "2100";
    public const string DoctorPayable = "2200";
    public const string SalaryPayable = "2300";
    // Equity
    public const string Capital = "3000";
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
        new(AccountCodes.MedicalEquipment, "Medical Equipment & Machines", "Asset", "Fixed Assets"),
        new(AccountCodes.Furniture, "Furniture & Fixtures", "Asset", "Fixed Assets"),
        new(AccountCodes.SupplierPayable, "Supplier Payable", "Liability", "Payables"),
        new(AccountCodes.StoreSupplierPayable, "Store Supplier Payable", "Liability", "Payables"),
        new(AccountCodes.CanteenSupplierPayable, "Canteen Supplier Payable", "Liability", "Payables"),
        new(AccountCodes.PatientAdvance, "Patient Advance", "Liability", "Payables"),
        new(AccountCodes.DoctorPayable, "Doctor Fee & Commission Payable", "Liability", "Payables"),
        new(AccountCodes.SalaryPayable, "Salary Payable", "Liability", "Payables"),
        new(AccountCodes.Capital, "Owner's Capital", "Equity", "Equity"),
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
    }

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

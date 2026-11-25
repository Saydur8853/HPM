using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

public static class OperationalAccounting
{
    /// <summary>One income line of a bill: the income head, the department, the gross amount.</summary>
    public sealed record RevenuePart(int IncomeAccountID, int? CostCenterID, decimal Gross);

    /// <summary>
    /// Posts (or re-posts when amounts changed) a bill: Dr Cash for the paid part, Dr Patient Receivable
    /// for the rest, Cr each income line gross by department, Dr Discount Allowed split over the same
    /// departments. Central cash collection is kept equal to the paid amount when a module flag is given.
    /// </summary>
    public static async Task SyncBillAsync(
        HpmDbContext db,
        string reference,
        long sourceId,
        IReadOnlyList<RevenuePart> parts,
        decimal discount,
        decimal cashAmount,
        DateTime transactionDate,
        int userId,
        string narration,
        int? cashModuleFlag = null)
    {
        var gross = parts.Sum(x => x.Gross);
        if (gross < 0 || discount < 0) throw new InvalidOperationException("Revenue amount cannot be negative.");
        discount = Math.Min(discount, gross);
        var net = gross - discount;
        if (cashAmount < 0 || cashAmount > net)
            throw new InvalidOperationException("Cash payment must be between zero and the net bill amount.");

        await FinancialControl.EnsurePostingAllowedAsync(db, transactionDate, userId);

        var lines = new List<AccountingPosting.PostingLine>();
        if (net > 0)
        {
            if (cashAmount > 0) lines.Add(new(AccountingPosting.Cash, cashAmount, 0));
            if (net - cashAmount > 0) lines.Add(new(AccountingPosting.PatientReceivable, net - cashAmount, 0));
            foreach (var g in parts.Where(x => x.Gross > 0).GroupBy(x => (x.IncomeAccountID, x.CostCenterID)))
                lines.Add(new(g.Key.IncomeAccountID, 0, g.Sum(x => x.Gross), g.Key.CostCenterID));
            if (discount > 0)
            {
                var discountId = await AccountMap.IncomeAsync(db, AccountMap.DiscountAllowed);
                foreach (var d in AccountingPosting.Split(discount, parts.Where(x => x.Gross > 0)
                             .GroupBy(x => x.CostCenterID).Select(g => (g.Key, g.Sum(x => x.Gross)))))
                    lines.Add(new(discountId, d.Amount, 0, d.Key));
            }
        }

        await AccountingPosting.SyncPostedVoucherAsync(db, reference, cashAmount > 0 ? "Receipt" : "Journal",
            transactionDate, narration, userId, lines.ToArray());

        if (cashModuleFlag.HasValue)
        {
            var collected = await db.CentralCashCollections
                .Where(x => x.ModuleFlag == cashModuleFlag && x.ReferenceID == sourceId)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
            var delta = cashAmount - collected;
            if (delta != 0)
                db.CentralCashCollections.Add(new CentralCashCollection
                {
                    Amount = delta,
                    ModuleFlag = cashModuleFlag.Value,
                    HeadID = cashModuleFlag.Value,
                    EnteredBy = userId,
                    TransDate = DateTime.Now,
                    TransDateCL = transactionDate,
                    ReferenceID = sourceId
                });
        }
    }

    /// <summary>Income lines of a diagnostic memo, one per test department (Pathology, Imaging, Cardiology).</summary>
    public static async Task<List<RevenuePart>> DiagnosticPartsAsync(
        HpmDbContext db, IEnumerable<(int TestID, decimal Rate, decimal Quantity)> items, decimal billGross)
    {
        var list = items.ToList();
        var centers = await AccountMap.DiagnosticCentersAsync(db, list.Select(x => x.TestID));
        var income = await AccountMap.IncomeAsync(db, AccountMap.DiagnosticIncome);
        var parts = list
            .GroupBy(x => centers.GetValueOrDefault(x.TestID))
            .Select(g => new RevenuePart(income, g.Key, g.Sum(x => x.Rate * x.Quantity)))
            .Where(x => x.Gross > 0)
            .ToList();
        // Bill total and item total can differ (rounding, legacy rows): keep the bill total exact.
        var diff = billGross - parts.Sum(x => x.Gross);
        if (diff != 0)
        {
            if (parts.Count == 0)
                parts.Add(new RevenuePart(income, await AccountMap.CenterAsync(db, AccountMap.CcPathology), billGross));
            else
            {
                var i = parts.FindIndex(x => x.Gross == parts.Max(y => y.Gross));
                parts[i] = parts[i] with { Gross = parts[i].Gross + diff };
            }
        }
        return parts;
    }

    /// <summary>Single-department bill (OPD ticket, Emergency, OT).</summary>
    public static async Task<List<RevenuePart>> SinglePartAsync(HpmDbContext db, string incomeCode, string centerCode, decimal gross) =>
        [new RevenuePart(await AccountMap.IncomeAsync(db, incomeCode), await AccountMap.CenterAsync(db, centerCode), gross)];

    /// <summary>Kept for callers that post a net amount to one department.</summary>
    public static async Task SyncSplitRevenueAsync(
        HpmDbContext db,
        string reference,
        long sourceId,
        decimal revenueAmount,
        decimal cashAmount,
        DateTime transactionDate,
        int userId,
        string narration,
        int? cashModuleFlag = null,
        string incomeCode = AccountMap.LegacyServiceRevenue,
        string? centerCode = null)
    {
        if (revenueAmount < 0) throw new InvalidOperationException("Revenue amount cannot be negative.");
        var parts = new List<RevenuePart>
        {
            new(await AccountMap.IncomeAsync(db, incomeCode), centerCode is null ? null : await AccountMap.CenterAsync(db, centerCode), revenueAmount)
        };
        await SyncBillAsync(db, reference, sourceId, parts, 0, cashAmount, transactionDate, userId, narration, cashModuleFlag);
    }

    /// <summary>Kept for callers that post the whole amount to Cash or to Patient Receivable.</summary>
    public static async Task SyncRevenueAsync(
        HpmDbContext db,
        string reference,
        long sourceId,
        decimal amount,
        DateTime transactionDate,
        int userId,
        string narration,
        int debitAccountId = AccountingPosting.Cash,
        int? cashModuleFlag = null,
        string incomeCode = AccountMap.LegacyServiceRevenue,
        string? centerCode = null)
    {
        if (amount < 0) throw new InvalidOperationException("Revenue amount cannot be negative.");
        var cash = debitAccountId == AccountingPosting.Cash ? amount : 0;
        var parts = new List<RevenuePart>
        {
            new(await AccountMap.IncomeAsync(db, incomeCode), centerCode is null ? null : await AccountMap.CenterAsync(db, centerCode), amount)
        };
        await SyncBillAsync(db, reference, sourceId, parts, 0, cash, transactionDate, userId, narration, cashModuleFlag);
    }

    /// <summary>
    /// Journal lines of an IPD final bill. Diagnostic tests and medicines given during the stay were
    /// already posted when they were ordered or sold, so the final bill only posts its own charges
    /// (admission, bed, hospital and other IPD services, ambulance) plus any difference between what
    /// the bill charges for tests/medicines and what was already posted for this admission. This
    /// keeps every taka of income in the ledger exactly once and Patient Receivable equal to the bill.
    /// </summary>
    public static async Task<AccountingPosting.PostingLine[]> IpdFinalBillLinesAsync(
        HpmDbContext db,
        string regNo,
        int? bedCabinId,
        decimal admissionFee,
        decimal ambulanceFee,
        decimal bedCharges,
        decimal hospitalCharges,
        IReadOnlyList<IpdPatientServiceItem> serviceItems,
        decimal pharmacyCharges,
        decimal discount)
    {
        static bool IsDiagnostic(IpdPatientServiceItem x) => x.Remarks != null &&
            (x.Remarks.StartsWith("[DIAGNOSTIC:", StringComparison.Ordinal) || x.Remarks.StartsWith("[DIAGNOSTIC-RETURN:", StringComparison.Ordinal));

        var bedCc = await AccountMap.BedCenterAsync(db, bedCabinId);
        var ipdIncome = await AccountMap.IncomeAsync(db, AccountMap.BedAdmissionIncome);
        var parts = new List<RevenuePart> { new(ipdIncome, bedCc, admissionFee + bedCharges + hospitalCharges) };
        if (ambulanceFee != 0)
            parts.Add(new(await AccountMap.IncomeAsync(db, AccountMap.OtherIncome), await AccountMap.CenterAsync(db, AccountMap.CcAmbulance), ambulanceFee));

        // Other IPD services: the department of the service category, otherwise the patient's ward.
        var otherItems = serviceItems.Where(x => !IsDiagnostic(x)).ToList();
        var serviceIds = otherItems.Select(x => x.ServiceID).Distinct().ToArray();
        var categoryCenter = await (from sv in db.Services
                                    join c in db.ServiceCategories on sv.CategoryID equals c.ID
                                    where serviceIds.Contains(sv.ID)
                                    select new { sv.ID, c.CostCenterID }).ToDictionaryAsync(x => x.ID, x => x.CostCenterID);
        parts.AddRange(otherItems
            .GroupBy(x => categoryCenter.GetValueOrDefault(x.ServiceID) ?? bedCc)
            .Select(g => new RevenuePart(ipdIncome, g.Key, g.Sum(x => x.Total))));

        // Tests and medicines: only the difference to what is already in the ledger for this patient.
        var (postedDiagnostic, postedPharmacy) = await PostedIpdReceivableAsync(db, regNo);
        var diagnosticItems = serviceItems.Where(IsDiagnostic).ToList();
        var diagnosticDifference = diagnosticItems.Sum(x => x.Total) - postedDiagnostic;
        if (diagnosticDifference != 0)
        {
            var centers = await AccountMap.DiagnosticCentersAsync(db, diagnosticItems.Select(x => x.ServiceID));
            var weights = diagnosticItems.GroupBy(x => centers.GetValueOrDefault(x.ServiceID))
                .Select(g => (g.Key, Math.Abs(g.Sum(x => x.Total)))).ToList();
            var income = await AccountMap.IncomeAsync(db, AccountMap.DiagnosticIncome);
            var split = AccountingPosting.Split(diagnosticDifference, weights);
            if (split.Count == 0) split = [(await AccountMap.CenterAsync(db, AccountMap.CcPathology), diagnosticDifference)];
            parts.AddRange(split.Select(x => new RevenuePart(income, x.Key, x.Amount)));
        }
        var pharmacyDifference = pharmacyCharges - postedPharmacy;
        if (pharmacyDifference != 0)
            parts.Add(new(await AccountMap.HeadAsync(db, AccountMap.PharmacySales, "Income", AccountingPosting.PharmacySales),
                await AccountMap.CenterAsync(db, AccountMap.CcPharmacy), pharmacyDifference));

        var lines = new List<AccountingPosting.PostingLine>();
        void Signed(int account, decimal debitMinusCredit, int? cc)
        {
            if (debitMinusCredit > 0) lines.Add(new(account, debitMinusCredit, 0, cc));
            else if (debitMinusCredit < 0) lines.Add(new(account, 0, -debitMinusCredit, cc));
        }

        foreach (var g in parts.GroupBy(x => (x.IncomeAccountID, x.CostCenterID)))
            Signed(g.Key.IncomeAccountID, -g.Sum(x => x.Gross), g.Key.CostCenterID);
        if (discount > 0)
        {
            var discountId = await AccountMap.IncomeAsync(db, AccountMap.DiscountAllowed);
            var split = AccountingPosting.Split(discount, parts.Where(x => x.Gross > 0 && x.IncomeAccountID == ipdIncome)
                .GroupBy(x => x.CostCenterID).Select(g => (g.Key, g.Sum(x => x.Gross))));
            if (split.Count == 0) split = [(bedCc, discount)];
            foreach (var d in split) Signed(discountId, d.Amount, d.Key);
        }
        Signed(AccountingPosting.PatientReceivable, parts.Sum(x => x.Gross) - discount, null);
        return lines.ToArray();
    }

    /// <summary>Patient Receivable already posted for an admission's diagnostic memos and pharmacy credit sales (incl. returns and reversals).</summary>
    public static async Task<(decimal Diagnostic, decimal Pharmacy)> PostedIpdReceivableAsync(HpmDbContext db, string regNo)
    {
        var diagnosticMemos = await db.EmergencyMemos.AsNoTracking().Where(x => x.RegNo == regNo && x.ModuleID == 2).Select(x => x.ID).ToListAsync();
        var diagnosticReturns = await db.DiagnosticCashMemoReturns.AsNoTracking().Where(x => diagnosticMemos.Contains(x.MemoID)).Select(x => x.ID).ToListAsync();
        var pharmacyMemos = await db.PharmacyCashMemos.AsNoTracking().Where(x => x.RegNo == regNo && x.PatientType == "2").Select(x => x.ID).ToListAsync();
        var pharmacyReturns = await db.PharmacySalesReturns.AsNoTracking().Where(x => pharmacyMemos.Contains(x.CashMemoID)).Select(x => x.ID).ToListAsync();

        async Task<decimal> ReceivableAsync(List<string> references)
        {
            if (references.Count == 0) return 0;
            var ids = await db.AccountVouchers.AsNoTracking()
                .Where(x => x.Reference != null && references.Contains(x.Reference) && x.IsPosted)
                .Select(x => x.ID).ToListAsync();
            if (ids.Count == 0) return 0;
            var reversalIds = await db.AccountVouchers.AsNoTracking()
                .Where(x => x.ReversalOfVoucherID != null && ids.Contains(x.ReversalOfVoucherID.Value))
                .Select(x => x.ID).ToListAsync();
            var all = ids.Concat(reversalIds).ToList();
            return await db.AccountVoucherLines.AsNoTracking()
                .Where(x => all.Contains(x.VoucherID) && x.AccountID == AccountingPosting.PatientReceivable)
                .SumAsync(x => (decimal?)(x.Debit - x.Credit)) ?? 0;
        }

        var diagnostic = await ReceivableAsync(diagnosticMemos.Select(x => $"DIAGNOSTIC:{x}")
            .Concat(diagnosticReturns.Select(x => $"DIAGNOSTIC-RETURN:{x}")).ToList());
        var pharmacy = await ReceivableAsync(pharmacyMemos.Select(x => $"PHARMACY-SALE:{x}")
            .Concat(pharmacyReturns.Select(x => $"PHARMACY-SALES-RETURN:{x}")).ToList());
        return (diagnostic, pharmacy);
    }

    public static async Task ReverseRevenueAsync(
        HpmDbContext db,
        string entityType,
        string reference,
        long sourceId,
        decimal amount,
        DateTime transactionDate,
        int userId,
        string reason,
        int debitAccountId = AccountingPosting.Cash,
        int? cashModuleFlag = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("A reversal reason is required.");
        if (await db.FinancialReversals.AnyAsync(x => x.EntityType == entityType && x.EntityID == sourceId))
            throw new InvalidOperationException("This source transaction is already reversed.");

        var reversedAt = DateTime.Now;
        await FinancialControl.EnsurePostingAllowedAsync(db, reversedAt, userId);
        var active = await db.AccountVouchers
            .Where(x => x.Reference == reference && x.IsPosted && x.Status != "Cancelled")
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync();
        if (active is not null)
            await AccountingPosting.ReversePostedVoucherAsync(db, active, reason, userId, reversedAt);

        if (cashModuleFlag.HasValue)
        {
            var collected = await db.CentralCashCollections
                .Where(x => x.ModuleFlag == cashModuleFlag && x.ReferenceID == sourceId)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
            if (collected != 0)
                db.CentralCashCollections.Add(new CentralCashCollection
                {
                    Amount = -collected,
                    ModuleFlag = cashModuleFlag.Value,
                    HeadID = cashModuleFlag.Value,
                    EnteredBy = userId,
                    TransDate = reversedAt,
                    TransDateCL = reversedAt,
                    ReferenceID = sourceId
                });
        }

        db.FinancialReversals.Add(new FinancialReversal
        {
            EntityType = entityType,
            EntityID = sourceId,
            Amount = amount,
            Reason = reason.Trim(),
            ReversedBy = userId,
            ReversedAt = reversedAt
        });
    }
}

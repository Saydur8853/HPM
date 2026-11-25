using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

/// <summary>
/// Resolves chart-of-accounts heads and cost centers by their code. Codes are stable across
/// hospitals while identity IDs are not. Results are cached per process (one hospital database per
/// container) and refreshed whenever a head or cost center is saved.
/// </summary>
public static class AccountMap
{
    // Account head codes used by automatic posting.
    public const string Cash = "1000";
    public const string Bank = "1020";
    public const string PatientReceivable = "1100";
    public const string MedicineInventory = "1200";
    public const string SupplierPayable = "2000";
    public const string PatientAdvance = "2100";
    public const string OwnersCapital = "3000";
    public const string RetainedEarnings = "3200";
    public const string ConsultationIncome = "4000";
    public const string LegacyServiceRevenue = "4090";
    public const string DiagnosticIncome = "4100";
    public const string BedAdmissionIncome = "4200";
    public const string ProcedureOtIncome = "4300";
    public const string PharmacySales = "4400";
    public const string OtherIncome = "4500";
    public const string DiscountAllowed = "4900";
    public const string CostOfMedicineSold = "5000";
    public const string OtherAdminExpense = "6900";
    public const string DepreciationExpense = "7000";

    // Cost center codes used by automatic posting.
    public const string CcOpd = "CC-101";
    public const string CcEmergency = "CC-102";
    public const string CcWard = "CC-103";
    public const string CcIcu = "CC-104";
    public const string CcOt = "CC-105";
    public const string CcPathology = "CC-106";
    public const string CcImaging = "CC-107";
    public const string CcCardiology = "CC-108";
    public const string CcPharmacy = "CC-109";
    public const string CcAmbulance = "CC-111";
    public const string CcGeneral = "CC-399";

    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static Dictionary<string, (int Id, string Type)>? _heads;
    private static Dictionary<string, int>? _centers;

    public static void Invalidate()
    {
        _heads = null;
        _centers = null;
    }

    private static async Task EnsureLoadedAsync(HpmDbContext db)
    {
        if (_heads is not null && _centers is not null) return;
        await Gate.WaitAsync();
        try
        {
            if (_heads is null)
                _heads = (await db.AccountHeads.AsNoTracking().Select(x => new { x.Code, x.ID, x.AccountType }).ToListAsync())
                    .ToDictionary(x => x.Code, x => (x.ID, x.AccountType), StringComparer.OrdinalIgnoreCase);
            if (_centers is null)
                _centers = (await db.CostCenters.AsNoTracking().Where(x => x.IsActive).Select(x => new { x.Code, x.ID }).ToListAsync())
                    .ToDictionary(x => x.Code, x => x.ID, StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            Gate.Release();
        }
    }

    /// <summary>
    /// ID of the head with this code. When the head is missing or is not of the expected type
    /// (a hospital re-used the code for something else) the fallback ID is returned, so posting
    /// never fails because of chart-of-accounts setup.
    /// </summary>
    public static async Task<int> HeadAsync(HpmDbContext db, string code, string expectedType, int fallbackId)
    {
        await EnsureLoadedAsync(db);
        return _heads!.TryGetValue(code, out var h) && string.Equals(h.Type, expectedType, StringComparison.OrdinalIgnoreCase)
            ? h.Id
            : fallbackId;
    }

    public static Task<int> IncomeAsync(HpmDbContext db, string code) =>
        HeadAsync(db, code, "Income", AccountingPosting.HospitalRevenue);

    /// <summary>ID of an active cost center with this code, or null.</summary>
    public static async Task<int?> CenterAsync(HpmDbContext db, string code)
    {
        await EnsureLoadedAsync(db);
        return _centers!.TryGetValue(code, out var id) ? id : null;
    }

    /// <summary>Default department for a diagnostic test when its category has no cost center set.</summary>
    public static string DiagnosticCenterCode(Service? service, ServiceCategory? category)
    {
        if (service is null) return CcPathology;
        if (DiagnosticTestRouting.IsImaging(service, category)) return CcImaging;
        if (DiagnosticTestRouting.IsProcedure(service, category)) return CcCardiology;
        return CcPathology;
    }

    /// <summary>
    /// Cost center for each diagnostic test: the category's own cost center when set,
    /// otherwise derived from the test type (pathology / imaging / procedure).
    /// </summary>
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
            result[id] = category?.CostCenterID ?? await CenterAsync(db, DiagnosticCenterCode(service, category));
        }
        return result;
    }

    /// <summary>Cost center of a bed category: its own setting, otherwise the general ward.</summary>
    public static async Task<int?> BedCenterAsync(HpmDbContext db, int? bedCabinId)
    {
        if (bedCabinId is int bedId)
        {
            var cc = await (from b in db.BedCabins
                            join c in db.BedCategories on b.CategoryID equals c.ID
                            where b.ID == bedId
                            select c.CostCenterID).FirstOrDefaultAsync();
            if (cc.HasValue) return cc;
        }
        return await CenterAsync(db, CcWard);
    }
}

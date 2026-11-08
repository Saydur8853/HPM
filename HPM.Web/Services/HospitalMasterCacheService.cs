using HPM.Web.Data;
using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HPM.Web.Services;

public sealed class HospitalMasterCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IDbContextFactory<HpmDbContext> _dbFactory;
    private static long _globalVersion = 1;

    private static readonly MemoryCacheEntryOptions DefaultCacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(30),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
    };

    public HospitalMasterCacheService(IMemoryCache cache, IDbContextFactory<HpmDbContext> dbFactory)
    {
        _cache = cache;
        _dbFactory = dbFactory;
    }

    private static string Key(string name) => $"hpm:master:v{Interlocked.Read(ref _globalVersion)}:{name}";

    /// <summary>
    /// Atomically invalidates all cached master lists when any master entity is added, updated, or deleted.
    /// </summary>
    public static void InvalidateGlobalCache() => Interlocked.Increment(ref _globalVersion);

    public void InvalidateAll() => InvalidateGlobalCache();

    public async Task<List<DoctorsProfile>> GetAllDoctorsAsync()
    {
        var list = await _cache.GetOrCreateAsync(Key("doctors:all"), async entry =>
        {
            entry.SetOptions(DefaultCacheOptions);
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Doctors.AsNoTracking()
                .OrderBy(x => x.Priority ?? int.MaxValue)
                .ThenBy(x => x.Name)
                .ToListAsync();
        });
        return list?.ToList() ?? [];
    }

    public async Task<List<DoctorsProfile>> GetDoctorsWithNameAsync(bool activeOnly = false)
    {
        var all = await GetAllDoctorsAsync();
        return all
            .Where(x => (!activeOnly || x.IsActive) && !string.IsNullOrEmpty(x.Name))
            .ToList();
    }

    public async Task<List<DoctorsProfile>> GetActiveDoctorsAsync()
    {
        var all = await GetAllDoctorsAsync();
        return all.Where(x => x.IsActive).ToList();
    }

    public async Task<List<Department>> GetAllDepartmentsAsync()
    {
        var list = await _cache.GetOrCreateAsync(Key("departments:all"), async entry =>
        {
            entry.SetOptions(DefaultCacheOptions);
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Departments.AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        });
        return list?.ToList() ?? [];
    }

    public async Task<List<Department>> GetActiveDepartmentsAsync()
    {
        var all = await GetAllDepartmentsAsync();
        return all.Where(x => x.IsActive).ToList();
    }

    public async Task<Dictionary<int, string>> GetDepartmentNamesMapAsync()
    {
        var all = await GetAllDepartmentsAsync();
        return all.ToDictionary(x => x.ID, x => x.Name);
    }

    public async Task<List<IpdDepartment>> GetIpdDepartmentsAsync()
    {
        var list = await _cache.GetOrCreateAsync(Key("ipd_departments:all"), async entry =>
        {
            entry.SetOptions(DefaultCacheOptions);
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.IpdDepartments.AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        });
        return list?.ToList() ?? [];
    }

    public async Task<List<BedCategory>> GetBedCategoriesAsync()
    {
        var list = await _cache.GetOrCreateAsync(Key("bed_categories:all"), async entry =>
        {
            entry.SetOptions(DefaultCacheOptions);
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.BedCategories.AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        });
        return list?.ToList() ?? [];
    }

    public async Task<List<TicketType>> GetActiveTicketTypesAsync()
    {
        var list = await _cache.GetOrCreateAsync(Key("ticket_types:active"), async entry =>
        {
            entry.SetOptions(DefaultCacheOptions);
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.TicketTypes.AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        });
        return list?.ToList() ?? [];
    }

    public async Task<(Dictionary<int, string> CategoryNames, List<Service> Services)> GetDiagnosticCatalogAsync()
    {
        var cached = await _cache.GetOrCreateAsync(Key("catalog:diagnostic"), async entry =>
        {
            entry.SetOptions(DefaultCacheOptions);
            await using var db = await _dbFactory.CreateDbContextAsync();
            var allCats = await db.ServiceCategories.AsNoTracking().ToListAsync();
            var categoryNames = allCats.ToDictionary(c => c.ID, c => c.Name);
            var diagCategoryIds = allCats
                .Where(c => c.SubID == 8
                         || c.ID == 66
                         || c.ID == 91
                         || (!string.IsNullOrEmpty(c.Name) && (c.Name.Contains("Investigation") || c.Name.Contains("Pathology"))))
                .Select(c => c.ID)
                .ToHashSet();

            var services = await db.Services.AsNoTracking()
                .Where(s => s.CategoryID != null
                         && diagCategoryIds.Contains(s.CategoryID.Value)
                         && s.CategoryID != 3
                         && s.CategoryID != 48
                         && s.CategoryID != 71
                         && s.CategoryID != 7
                         && (s.GenericID == null || s.GenericID == 0)
                         && (s.TypeID == null || s.TypeID == 0)
                         && (s.SupplierID == null || s.SupplierID == 0))
                .OrderBy(x => x.Name)
                .ToListAsync();
            return (CategoryNames: categoryNames, Services: services);
        });

        return (new Dictionary<int, string>(cached.CategoryNames), cached.Services.ToList());
    }

    public async Task<(List<ServiceCategory> Categories, List<Service> Services)> GetOtCatalogAsync()
    {
        var cached = await _cache.GetOrCreateAsync(Key("catalog:ot"), async entry =>
        {
            entry.SetOptions(DefaultCacheOptions);
            await using var db = await _dbFactory.CreateDbContextAsync();
            var categories = await db.ServiceCategories.AsNoTracking()
                .Where(x => x.ID == 1 || x.SubID == 1)
                .OrderBy(x => x.Name)
                .ToListAsync();
            var categoryIds = categories.Select(x => x.ID).ToHashSet();
            var services = await db.Services.AsNoTracking()
                .Where(x => x.CategoryID != null && categoryIds.Contains(x.CategoryID.Value))
                .OrderBy(x => x.Name)
                .ToListAsync();
            return (Categories: categories, Services: services);
        });

        return (cached.Categories.ToList(), cached.Services.ToList());
    }

    public async Task<(Dictionary<int, string> GenericNames, Dictionary<int, string> TypeNames, List<Service> Medicines)> GetPharmacyMedicineCatalogAsync()
    {
        var cached = await _cache.GetOrCreateAsync(Key("catalog:pharmacy"), async entry =>
        {
            entry.SetOptions(DefaultCacheOptions);
            await using var db = await _dbFactory.CreateDbContextAsync();
            var genericNames = await db.GenericGroups.AsNoTracking()
                .Where(x => x.Name != null && x.Name != "")
                .ToDictionaryAsync(x => x.ID, x => x.Name!);
            var typeNames = await db.MedicineTypes.AsNoTracking()
                .ToDictionaryAsync(x => x.ID, x => x.Type);
            var medicines = await db.Services.AsNoTracking()
                .Where(x => x.CategoryID == 3 && x.Name != null && x.Name != "")
                .OrderBy(x => x.Name)
                .ToListAsync();
            return (GenericNames: genericNames, TypeNames: typeNames, Medicines: medicines);
        });

        return (new Dictionary<int, string>(cached.GenericNames), new Dictionary<int, string>(cached.TypeNames), cached.Medicines.ToList());
    }
}

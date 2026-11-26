using HPM.Web.Data;
using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Services;

/// <summary>Item row shown in the store item picker (Stock is null when stock is irrelevant, e.g. GRN).</summary>
public sealed record StorePickItem(int Id, string Name, string Unit, string Category, decimal? Stock, int ReorderLevel, bool TrackExpiry, decimal BuyRate);

/// <summary>Reference data shared by the hospital store pages.</summary>
public sealed class StoreLookups
{
    public List<StoreLocation> Locations { get; private init; } = [];
    public List<StoreDepartment> Departments { get; private init; } = [];
    public List<StoreProductCategory> Categories { get; private init; } = [];
    public List<SupplierStore> Suppliers { get; private init; } = [];
    public List<StoreProduct> Products { get; private init; } = [];
    /// <summary>Usable (non-expired) stock per (location, product).</summary>
    public Dictionary<(int Location, int Product), decimal> Stock { get; private init; } = [];

    public static async Task<StoreLookups> LoadAsync(HpmDbContext db, bool includeInactive = false)
    {
        var today = DateTime.Today;
        var stock = await db.StoreStocks.AsNoTracking()
            .Where(s => s.Balance > 0 && (s.ExpireDate == null || s.ExpireDate >= today))
            .GroupBy(s => new { s.LocationID, s.ProductID })
            .Select(g => new { g.Key.LocationID, g.Key.ProductID, Qty = g.Sum(x => x.Balance) })
            .ToListAsync();
        return new StoreLookups
        {
            Locations = await db.StoreLocations.AsNoTracking().Where(x => includeInactive || x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToListAsync(),
            Departments = await db.StoreDepartments.AsNoTracking().Where(x => includeInactive || x.IsActive).OrderBy(x => x.Name).ToListAsync(),
            Categories = await db.StoreProductCategories.AsNoTracking().OrderBy(x => x.Name).ToListAsync(),
            Suppliers = await db.StoreSuppliers.AsNoTracking().OrderBy(x => x.Name).ToListAsync(),
            Products = await db.StoreProducts.AsNoTracking().Where(x => includeInactive || x.IsActive).OrderBy(x => x.Name).ToListAsync(),
            Stock = stock.ToDictionary(x => (x.LocationID, x.ProductID), x => x.Qty)
        };
    }

    public int DefaultLocationId => Locations.FirstOrDefault(x => x.IsCentral)?.ID ?? Locations.FirstOrDefault()?.ID ?? 0;

    public string LocationName(int? id) => Locations.FirstOrDefault(x => x.ID == id)?.Name ?? "—";
    public string DepartmentName(int? id) => Departments.FirstOrDefault(x => x.ID == id)?.Name ?? "—";
    public string ProductName(int? id) => Products.FirstOrDefault(x => x.ID == id)?.Name ?? "—";
    public string ProductUnit(int? id) => Products.FirstOrDefault(x => x.ID == id)?.Unit ?? "";
    public string CategoryName(int id) => Categories.FirstOrDefault(x => x.ID == id)?.Name ?? "";
    public string SupplierName(int id) => Suppliers.FirstOrDefault(x => x.ID == id)?.Name ?? "—";
    public decimal StockAt(int locationId, int productId) => Stock.GetValueOrDefault((locationId, productId));

    /// <summary>Picker rows; pass a location to show usable stock there, or null to hide stock.</summary>
    public List<StorePickItem> PickItems(int? locationId) => Products
        .Select(p => new StorePickItem(p.ID, p.Name, p.Unit, CategoryName(p.CategoryID), locationId.HasValue ? StockAt(locationId.Value, p.ID) : null,
            p.ReOrderLevel, p.TrackExpiry, p.BuyRate))
        .ToList();
}

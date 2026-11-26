using System.Data;
using HPM.Web.Data;
using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Services;

public sealed record StoreLineInput(int ProductId, decimal Qty);
public sealed record StoreGrnLineInput(int ProductId, decimal Qty, decimal Rate, string? BatchNo, DateTime? ExpireDate);
public sealed record StoreStockLineInput(int StockId, decimal Qty);
public sealed record StoreAdjustLineInput(int StockId, decimal NewQty);
public sealed record StoreReturnLineInput(int ProductId, decimal Qty, bool Restock);

/// <summary>
/// All hospital-store stock movements go through here so batch balances (Store_Stock), the movement
/// ledger (Store_Stock_Ledger) and the cached item total (Store_Product.Balance) always agree.
/// Every operation runs in one serializable transaction.
/// </summary>
public sealed class StoreInventoryService(IDbContextFactory<HpmDbContext> dbFactory)
{
    public const string RefOpening = "OPENING";
    public const string RefGrn = "GRN";
    public const string RefIssue = "ISSUE";
    public const string RefReturn = "RETURN";
    public const string RefTransferIn = "TRANSFER_IN";
    public const string RefTransferOut = "TRANSFER_OUT";
    public const string RefAdjust = "ADJUST";

    // ---------------- Goods receive (GRN) ----------------
    public Task<StoreChallan> ReceiveAsync(int supplierId, string invoiceNo, DateTime invoiceDate, int locationId, string? note,
        IReadOnlyList<StoreGrnLineInput> lines, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            var clean = lines.Where(x => x.Qty > 0).ToList();
            if (clean.Count == 0) throw new InvalidOperationException("Add at least one item with quantity.");
            if (clean.Any(x => x.Rate < 0)) throw new InvalidOperationException("Rate cannot be negative.");
            await EnsureLocationAsync(db, locationId);
            var products = await LoadProductsAsync(db, clean.Select(x => x.ProductId));
            foreach (var l in clean)
                if (products[l.ProductId].TrackExpiry && l.ExpireDate == null)
                    throw new InvalidOperationException($"Expiry date is required for {products[l.ProductId].Name}.");

            var now = DateTime.Now;
            var challan = new StoreChallan
            {
                Supplier = supplierId,
                InvoiceNo = Trunc(invoiceNo.Trim(), 20),
                InvoiceDate = invoiceDate,
                IsPosted = true,
                Total = clean.Sum(x => Math.Round(x.Qty * x.Rate, 2)),
                EnteredBy = userId,
                TransDate = now,
                LocationID = locationId,
                Note = TruncOrNull(note, 300)
            };
            db.StoreChallans.Add(challan);
            await db.SaveChangesAsync();

            var sl = 1;
            foreach (var l in clean)
            {
                var batch = TruncOrNull(l.BatchNo, 50);
                var expiry = l.ExpireDate?.Date;
                var stock = await db.StoreStocks.FirstOrDefaultAsync(s => s.LocationID == locationId && s.ProductID == l.ProductId
                    && s.BatchNo == batch && s.ExpireDate == expiry && s.BuyRate == l.Rate);
                if (stock == null)
                {
                    stock = new StoreStock { LocationID = locationId, ProductID = l.ProductId, BatchNo = batch, ExpireDate = expiry, BuyRate = l.Rate, ReceivedDate = now };
                    db.StoreStocks.Add(stock);
                    await db.SaveChangesAsync();
                }
                stock.Balance += l.Qty;
                products[l.ProductId].BuyRate = l.Rate;
                db.StoreChallanItems.Add(new StoreChallanSub
                {
                    ChallanNo = challan.ID, ProductID = l.ProductId, SLNO = sl++, Quantity = l.Qty, BuyRate = l.Rate,
                    Total = Math.Round(l.Qty * l.Rate, 2), BatchNo = batch, ExpireDate = expiry, StockID = stock.ID
                });
                db.StoreStockLedger.Add(Ledger(now, locationId, l.ProductId, stock.ID, l.Qty, 0, l.Rate, RefGrn, challan.ID, null, $"Invoice {challan.InvoiceNo}", userId));
            }
            await SaveAndRecalcAsync(db, products.Values);
            return challan;
        }, IsolationLevel.Serializable);

    // ---------------- Issue to department (FEFO) ----------------
    public Task<StoreIssue> IssueAsync(int departmentId, int locationId, long? requisitionId, string? receivedBy, string? note,
        IReadOnlyList<StoreLineInput> lines, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            var clean = lines.Where(x => x.Qty > 0).GroupBy(x => x.ProductId).Select(g => new StoreLineInput(g.Key, g.Sum(x => x.Qty))).ToList();
            if (clean.Count == 0) throw new InvalidOperationException("Add at least one item with quantity.");
            if (departmentId <= 0) throw new InvalidOperationException("Select the receiving department.");
            await EnsureLocationAsync(db, locationId);
            var products = await LoadProductsAsync(db, clean.Select(x => x.ProductId));

            StoreRequisition? req = null;
            if (requisitionId.HasValue)
            {
                req = await db.StoreRequisitions.Include(x => x.Items).FirstOrDefaultAsync(x => x.ID == requisitionId.Value)
                      ?? throw new InvalidOperationException("Requisition not found.");
                if (req.Status is not (StoreStatus.Approved or StoreStatus.PartiallyIssued))
                    throw new InvalidOperationException($"Requisition #{req.ID} is {req.Status}; only approved requisitions can be issued.");
                foreach (var l in clean)
                {
                    var line = req.Items.FirstOrDefault(x => x.ProductID == l.ProductId)
                               ?? throw new InvalidOperationException($"{products[l.ProductId].Name} is not in requisition #{req.ID}.");
                    var remaining = (line.QtyApproved ?? line.QtyRequested) - line.QtyIssued;
                    if (l.Qty > remaining)
                        throw new InvalidOperationException($"{products[l.ProductId].Name}: only {remaining:0.##} remaining on the requisition.");
                }
            }

            var now = DateTime.Now;
            var issue = new StoreIssue
            {
                IssueDepartment = departmentId,
                IndentNo = req != null ? $"REQ-{req.ID}" : "DIRECT",
                EnteredBy = userId,
                TransDate = now,
                LocationID = locationId,
                RequisitionID = req?.ID,
                ReceivedBy = TruncOrNull(receivedBy, 100),
                Note = TruncOrNull(note, 300)
            };
            db.StoreIssues.Add(issue);
            await db.SaveChangesAsync();

            var sl = 1;
            decimal total = 0;
            var today = DateTime.Today;
            foreach (var l in clean)
            {
                var batches = await db.StoreStocks
                    .Where(s => s.LocationID == locationId && s.ProductID == l.ProductId && s.Balance > 0 && (s.ExpireDate == null || s.ExpireDate >= today))
                    .OrderBy(s => s.ExpireDate == null).ThenBy(s => s.ExpireDate).ThenBy(s => s.ReceivedDate).ThenBy(s => s.ID)
                    .ToListAsync();
                var available = batches.Sum(b => b.Balance);
                if (available < l.Qty)
                    throw new InvalidOperationException($"{products[l.ProductId].Name}: only {available:0.##} {products[l.ProductId].Unit} in stock (non-expired) at this store.");
                var remaining = l.Qty;
                foreach (var b in batches)
                {
                    if (remaining <= 0) break;
                    var take = Math.Min(b.Balance, remaining);
                    b.Balance -= take;
                    remaining -= take;
                    var lineTotal = Math.Round(take * b.BuyRate, 2);
                    total += lineTotal;
                    db.StoreIssueItems.Add(new StoreIssueSub
                    {
                        IssueNo = issue.ID, ProductID = l.ProductId, SLNO = sl++, BuyRate = b.BuyRate, Quantity = take, Total = lineTotal,
                        StockID = b.ID, BatchNo = b.BatchNo, ExpireDate = b.ExpireDate
                    });
                    db.StoreStockLedger.Add(Ledger(now, locationId, l.ProductId, b.ID, 0, take, b.BuyRate, RefIssue, issue.ID, departmentId, issue.IndentNo, userId));
                }
                if (req != null) req.Items.First(x => x.ProductID == l.ProductId).QtyIssued += l.Qty;
            }
            issue.Total = total;
            if (req != null)
            {
                var allDone = req.Items.All(x => x.QtyIssued >= (x.QtyApproved ?? x.QtyRequested));
                req.Status = allDone ? StoreStatus.Issued : StoreStatus.PartiallyIssued;
            }
            await SaveAndRecalcAsync(db, products.Values);
            return issue;
        }, IsolationLevel.Serializable);

    // ---------------- Department return ----------------
    public Task<StoreMovement> ReturnAsync(int departmentId, int locationId, string? reason, string? note,
        IReadOnlyList<StoreReturnLineInput> lines, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            var clean = lines.Where(x => x.Qty > 0).ToList();
            if (clean.Count == 0) throw new InvalidOperationException("Add at least one item with quantity.");
            if (departmentId <= 0) throw new InvalidOperationException("Select the returning department.");
            await EnsureLocationAsync(db, locationId);
            var products = await LoadProductsAsync(db, clean.Select(x => x.ProductId));
            var now = DateTime.Now;
            var mv = new StoreMovement { Type = "RETURN", LocationID = locationId, DepartmentID = departmentId, Reason = TruncOrNull(reason, 40), Note = TruncOrNull(note, 300), EnteredBy = userId, TransDate = now };
            db.StoreMovements.Add(mv);
            await db.SaveChangesAsync();

            var today = DateTime.Today;
            foreach (var l in clean)
            {
                var product = products[l.ProductId];
                StoreStock? stock = null;
                if (l.Restock)
                {
                    stock = await db.StoreStocks
                        .Where(s => s.LocationID == locationId && s.ProductID == l.ProductId && (s.ExpireDate == null || s.ExpireDate >= today))
                        .OrderByDescending(s => s.ReceivedDate).ThenByDescending(s => s.ID).FirstOrDefaultAsync();
                    if (stock == null)
                    {
                        stock = new StoreStock { LocationID = locationId, ProductID = l.ProductId, BatchNo = "RETURN", BuyRate = product.BuyRate, ReceivedDate = now };
                        db.StoreStocks.Add(stock);
                        await db.SaveChangesAsync();
                    }
                    stock.Balance += l.Qty;
                    db.StoreStockLedger.Add(Ledger(now, locationId, l.ProductId, stock.ID, l.Qty, 0, stock.BuyRate, RefReturn, mv.ID, departmentId, mv.Reason, userId));
                }
                db.StoreMovementItems.Add(new StoreMovementSub
                {
                    MovementID = mv.ID, ProductID = l.ProductId, StockID = stock?.ID, BatchNo = stock?.BatchNo, ExpireDate = stock?.ExpireDate,
                    Quantity = l.Qty, Rate = stock?.BuyRate ?? product.BuyRate, IsRestocked = l.Restock
                });
            }
            await SaveAndRecalcAsync(db, products.Values);
            return mv;
        }, IsolationLevel.Serializable);

    // ---------------- Transfer between sub-stores ----------------
    public Task<StoreMovement> TransferAsync(int fromLocationId, int toLocationId, string? note, IReadOnlyList<StoreStockLineInput> lines, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            if (fromLocationId == toLocationId) throw new InvalidOperationException("Source and destination store must be different.");
            var clean = lines.Where(x => x.Qty > 0).ToList();
            if (clean.Count == 0) throw new InvalidOperationException("Add at least one batch with quantity.");
            await EnsureLocationAsync(db, fromLocationId);
            await EnsureLocationAsync(db, toLocationId);
            var now = DateTime.Now;
            var mv = new StoreMovement { Type = "TRANSFER", LocationID = fromLocationId, ToLocationID = toLocationId, Note = TruncOrNull(note, 300), EnteredBy = userId, TransDate = now };
            db.StoreMovements.Add(mv);
            await db.SaveChangesAsync();

            var touched = new List<StoreProduct>();
            foreach (var l in clean)
            {
                var src = await db.StoreStocks.FirstOrDefaultAsync(s => s.ID == l.StockId && s.LocationID == fromLocationId)
                          ?? throw new InvalidOperationException("Selected batch was not found in the source store.");
                var product = await db.StoreProducts.FirstAsync(p => p.ID == src.ProductID);
                touched.Add(product);
                if (l.Qty > src.Balance) throw new InvalidOperationException($"{product.Name}: only {src.Balance:0.##} in batch {src.BatchNo ?? "-"}.");
                src.Balance -= l.Qty;
                var dest = await db.StoreStocks.FirstOrDefaultAsync(s => s.LocationID == toLocationId && s.ProductID == src.ProductID
                    && s.BatchNo == src.BatchNo && s.ExpireDate == src.ExpireDate && s.BuyRate == src.BuyRate);
                if (dest == null)
                {
                    dest = new StoreStock { LocationID = toLocationId, ProductID = src.ProductID, BatchNo = src.BatchNo, ExpireDate = src.ExpireDate, BuyRate = src.BuyRate, ReceivedDate = now };
                    db.StoreStocks.Add(dest);
                    await db.SaveChangesAsync();
                }
                dest.Balance += l.Qty;
                db.StoreMovementItems.Add(new StoreMovementSub { MovementID = mv.ID, ProductID = src.ProductID, StockID = src.ID, BatchNo = src.BatchNo, ExpireDate = src.ExpireDate, Quantity = l.Qty, Rate = src.BuyRate });
                db.StoreStockLedger.Add(Ledger(now, fromLocationId, src.ProductID, src.ID, 0, l.Qty, src.BuyRate, RefTransferOut, mv.ID, null, null, userId));
                db.StoreStockLedger.Add(Ledger(now, toLocationId, src.ProductID, dest.ID, l.Qty, 0, src.BuyRate, RefTransferIn, mv.ID, null, null, userId));
            }
            await SaveAndRecalcAsync(db, touched);
            return mv;
        }, IsolationLevel.Serializable);

    // ---------------- Stock count / adjustment ----------------
    public Task<StoreMovement> AdjustAsync(int locationId, string reason, string? note, IReadOnlyList<StoreAdjustLineInput> lines, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            if (lines.Any(x => x.NewQty < 0)) throw new InvalidOperationException("Counted quantity cannot be negative.");
            await EnsureLocationAsync(db, locationId);
            var now = DateTime.Now;
            var mv = new StoreMovement { Type = "ADJUST", LocationID = locationId, Reason = TruncOrNull(reason, 40), Note = TruncOrNull(note, 300), EnteredBy = userId, TransDate = now };
            var touched = new List<StoreProduct>();
            var subs = new List<(StoreStock Stock, decimal Before, decimal Diff)>();
            foreach (var l in lines)
            {
                var stock = await db.StoreStocks.FirstOrDefaultAsync(s => s.ID == l.StockId && s.LocationID == locationId)
                            ?? throw new InvalidOperationException("Selected batch was not found in this store.");
                var diff = l.NewQty - stock.Balance;
                if (diff == 0) continue;
                subs.Add((stock, stock.Balance, diff));
                stock.Balance = l.NewQty;
                touched.Add(await db.StoreProducts.FirstAsync(p => p.ID == stock.ProductID));
            }
            if (subs.Count == 0) throw new InvalidOperationException("No quantity was changed.");
            db.StoreMovements.Add(mv);
            await db.SaveChangesAsync();
            foreach (var (stock, before, diff) in subs)
            {
                db.StoreMovementItems.Add(new StoreMovementSub { MovementID = mv.ID, ProductID = stock.ProductID, StockID = stock.ID, BatchNo = stock.BatchNo, ExpireDate = stock.ExpireDate, Quantity = diff, QtyBefore = before, Rate = stock.BuyRate });
                db.StoreStockLedger.Add(Ledger(now, locationId, stock.ProductID, stock.ID, diff > 0 ? diff : 0, diff < 0 ? -diff : 0, stock.BuyRate, RefAdjust, mv.ID, null, mv.Reason, userId));
            }
            await SaveAndRecalcAsync(db, touched);
            return mv;
        }, IsolationLevel.Serializable);

    // ---------------- Opening stock for a newly created item ----------------
    public Task<StoreStock> OpeningAsync(int productId, int locationId, decimal qty, decimal rate, string? batchNo, DateTime? expireDate, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            if (qty <= 0) throw new InvalidOperationException("Opening quantity must be more than zero.");
            await EnsureLocationAsync(db, locationId);
            var products = await LoadProductsAsync(db, [productId]);
            if (products[productId].TrackExpiry && expireDate == null) throw new InvalidOperationException("Expiry date is required for this item.");
            var now = DateTime.Now;
            var stock = new StoreStock { LocationID = locationId, ProductID = productId, BatchNo = TruncOrNull(batchNo, 50) ?? "OPENING", ExpireDate = expireDate?.Date, Balance = qty, BuyRate = rate, ReceivedDate = now };
            db.StoreStocks.Add(stock);
            await db.SaveChangesAsync();
            db.StoreStockLedger.Add(Ledger(now, locationId, productId, stock.ID, qty, 0, rate, RefOpening, 0, null, "Opening stock", userId));
            await SaveAndRecalcAsync(db, products.Values);
            return stock;
        }, IsolationLevel.Serializable);

    // ---------------- Requisitions ----------------
    public Task<StoreRequisition> CreateRequisitionAsync(int departmentId, int locationId, string priority, string? note,
        IReadOnlyList<StoreLineInput> lines, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            var clean = lines.Where(x => x.Qty > 0).GroupBy(x => x.ProductId).Select(g => new StoreLineInput(g.Key, g.Sum(x => x.Qty))).ToList();
            if (clean.Count == 0) throw new InvalidOperationException("Add at least one item with quantity.");
            if (departmentId <= 0) throw new InvalidOperationException("Select your department.");
            await EnsureLocationAsync(db, locationId);
            await LoadProductsAsync(db, clean.Select(x => x.ProductId));
            var req = new StoreRequisition
            {
                DepartmentID = departmentId, LocationID = locationId, Status = StoreStatus.Pending,
                Priority = priority == "Urgent" ? "Urgent" : "Normal", Note = TruncOrNull(note, 300),
                RequestedBy = userId, RequestedAt = DateTime.Now,
                Items = clean.Select(x => new StoreRequisitionSub { ProductID = x.ProductId, QtyRequested = x.Qty }).ToList()
            };
            db.StoreRequisitions.Add(req);
            await db.SaveChangesAsync();
            return req;
        });

    /// <summary>Approve (with per-line approved quantities, 0 = not approved) or reject a pending requisition.</summary>
    public Task<StoreRequisition> ReviewRequisitionAsync(long id, bool approve, IReadOnlyDictionary<long, decimal> approvedQty, string? note, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            var req = await db.StoreRequisitions.Include(x => x.Items).FirstOrDefaultAsync(x => x.ID == id)
                      ?? throw new InvalidOperationException("Requisition not found.");
            if (req.Status != StoreStatus.Pending) throw new InvalidOperationException($"Requisition #{id} is already {req.Status}.");
            if (approve)
            {
                foreach (var line in req.Items)
                {
                    var qty = approvedQty.TryGetValue(line.ID, out var q) ? q : line.QtyRequested;
                    if (qty < 0) throw new InvalidOperationException("Approved quantity cannot be negative.");
                    line.QtyApproved = qty;
                }
                if (req.Items.All(x => (x.QtyApproved ?? 0) <= 0)) throw new InvalidOperationException("Approve at least one item, or reject the requisition.");
            }
            req.Status = approve ? StoreStatus.Approved : StoreStatus.Rejected;
            req.ReviewedBy = userId;
            req.ReviewedAt = DateTime.Now;
            req.ReviewNote = TruncOrNull(note, 300);
            await db.SaveChangesAsync();
            return req;
        });

    public Task<StoreRequisition> CancelRequisitionAsync(long id, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            var req = await db.StoreRequisitions.FirstOrDefaultAsync(x => x.ID == id) ?? throw new InvalidOperationException("Requisition not found.");
            if (req.Status is not (StoreStatus.Pending or StoreStatus.Approved))
                throw new InvalidOperationException($"Requisition #{id} is {req.Status} and cannot be cancelled.");
            if (req.Status == StoreStatus.Approved && await db.StoreIssues.AnyAsync(x => x.RequisitionID == id))
                throw new InvalidOperationException("Items were already issued against this requisition.");
            req.Status = StoreStatus.Cancelled;
            req.ReviewedBy ??= userId;
            req.ReviewedAt ??= DateTime.Now;
            await db.SaveChangesAsync();
            return req;
        });

    /// <summary>Requisition status after a full or partial issue (used when closing a partially issued one).</summary>
    public Task CloseRequisitionAsync(long id, int userId) =>
        ResilientTransaction.ExecuteAsync(dbFactory, async db =>
        {
            var req = await db.StoreRequisitions.FirstOrDefaultAsync(x => x.ID == id) ?? throw new InvalidOperationException("Requisition not found.");
            if (req.Status != StoreStatus.PartiallyIssued) throw new InvalidOperationException("Only partially issued requisitions can be closed.");
            req.Status = StoreStatus.Issued;
            req.ReviewNote = TruncOrNull($"{req.ReviewNote} [Closed short]".Trim(), 300);
            await db.SaveChangesAsync();
            return true;
        });

    // ---------------- helpers ----------------
    private static async Task EnsureLocationAsync(HpmDbContext db, int locationId)
    {
        if (!await db.StoreLocations.AnyAsync(x => x.ID == locationId && x.IsActive))
            throw new InvalidOperationException("Select an active store.");
    }

    private static async Task<Dictionary<int, StoreProduct>> LoadProductsAsync(HpmDbContext db, IEnumerable<int> ids)
    {
        var idList = ids.Distinct().ToList();
        var map = await db.StoreProducts.Where(p => idList.Contains(p.ID)).ToDictionaryAsync(p => p.ID);
        var missing = idList.Where(id => !map.ContainsKey(id)).ToList();
        if (missing.Count > 0) throw new InvalidOperationException("One or more items no longer exist.");
        return map;
    }

    /// <summary>Saves, then refreshes the cached Store_Product.Balance from batch stock for the touched items.</summary>
    private static async Task SaveAndRecalcAsync(HpmDbContext db, IEnumerable<StoreProduct> products)
    {
        await db.SaveChangesAsync();
        foreach (var p in products.DistinctBy(x => x.ID))
            p.Balance = await db.StoreStocks.Where(s => s.ProductID == p.ID).SumAsync(s => (decimal?)s.Balance) ?? 0;
        await db.SaveChangesAsync();
    }

    private static StoreStockLedger Ledger(DateTime at, int locationId, int productId, int? stockId, decimal qtyIn, decimal qtyOut, decimal rate,
        string refType, long refId, int? departmentId, string? note, int userId) => new()
    {
        TransDate = at, LocationID = locationId, ProductID = productId, StockID = stockId, QtyIn = qtyIn, QtyOut = qtyOut,
        Rate = rate, RefType = refType, RefID = refId, DepartmentID = departmentId, Note = TruncOrNull(note, 200), EnteredBy = userId
    };

    private static string Trunc(string value, int max) => value.Length <= max ? value : value[..max];
    private static string? TruncOrNull(string? value, int max) => string.IsNullOrWhiteSpace(value) ? null : Trunc(value.Trim(), max);
}

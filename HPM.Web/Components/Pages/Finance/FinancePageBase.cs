using System.Security.Claims;
using HPM.Web.Data;
using HPM.Web.Data.Entities;
using HPM.Web.Localization;
using HPM.Web.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace HPM.Web.Components.Pages.Finance;

/// <summary>Shared plumbing for the Accounts &amp; Finance pages.</summary>
public abstract class FinancePageBase : ComponentBase, IDisposable
{
    [Inject] protected IDbContextFactory<HpmDbContext> DbFactory { get; set; } = default!;
    [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected I18nService Localizer { get; set; } = default!;

    protected int CurrentUserId { get; private set; }
    protected bool IsAdmin { get; private set; }
    protected List<AccountHead> Heads { get; set; } = [];
    protected string Message { get; set; } = "";
    protected string MessageClass { get; set; } = "success";
    protected bool IsBusy { get; set; }

    protected DateTime FromDate { get; set; } = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    protected DateTime ToDate { get; set; } = DateTime.Today;
    protected string Preset { get; set; } = "month";

    protected override async Task OnInitializedAsync()
    {
        Localizer.OnLanguageChanged += HandleLanguageChanged;
        var user = (await AuthStateProvider.GetAuthenticationStateAsync()).User;
        CurrentUserId = int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;
        // Finance authority: Administrator (incl. Hospital Owner) or Accountant.
        IsAdmin = user.IsInRole(RoleNames.Administrator) || user.IsInRole(RoleNames.Accountant);
        await using (var db = await DbFactory.CreateDbContextAsync())
            Heads = await db.AccountHeads.AsNoTracking().OrderBy(x => x.Code).ToListAsync();
        await OnFinanceInitializedAsync();
    }

    protected virtual Task OnFinanceInitializedAsync() => Task.CompletedTask;
    protected virtual Task OnRangeChangedAsync() => Task.CompletedTask;

    protected AccountHead? Head(string code) => Heads.FirstOrDefault(x => x.Code == code);
    protected string HeadName(int id) => Heads.FirstOrDefault(x => x.ID == id)?.Name ?? $"#{id}";
    protected string HeadCode(int id) => Heads.FirstOrDefault(x => x.ID == id)?.Code ?? "";

    protected async Task ApplyPresetAsync(string preset)
    {
        Preset = preset;
        var t = DateTime.Today;
        (FromDate, ToDate) = preset switch
        {
            "today" => (t, t),
            "7days" => (t.AddDays(-6), t),
            "lastmonth" => (new DateTime(t.Year, t.Month, 1).AddMonths(-1), new DateTime(t.Year, t.Month, 1).AddDays(-1)),
            "year" => (new DateTime(t.Year, 1, 1), t),
            _ => (new DateTime(t.Year, t.Month, 1), t)
        };
        await OnRangeChangedAsync();
    }

    protected async Task SetFromAsync(ChangeEventArgs e)
    {
        if (!DateTime.TryParse(e.Value?.ToString(), out var d)) return;
        FromDate = d.Date; Preset = "custom";
        if (FromDate > ToDate) ToDate = FromDate;
        await OnRangeChangedAsync();
    }

    protected async Task SetToAsync(ChangeEventArgs e)
    {
        if (!DateTime.TryParse(e.Value?.ToString(), out var d)) return;
        ToDate = d.Date; Preset = "custom";
        if (ToDate < FromDate) FromDate = ToDate;
        await OnRangeChangedAsync();
    }

    /// <summary>Posted lines (approved, cancelled-with-reversal) between two dates, joined to their voucher.</summary>
    protected static async Task<List<PostedLine>> PostedLinesAsync(HpmDbContext db, DateTime? from, DateTime to)
    {
        var end = to.Date.AddDays(1);
        // A reversal carries the reference of the voucher it undoes, so it nets against the same source.
        var q = from l in db.AccountVoucherLines.AsNoTracking()
                join v in db.AccountVouchers.AsNoTracking() on l.VoucherID equals v.ID
                join o in db.AccountVouchers.AsNoTracking() on v.ReversalOfVoucherID equals (long?)o.ID into oj
                from o in oj.DefaultIfEmpty()
                where v.IsPosted && v.Status != "Pending" && v.Status != "Rejected" && v.VoucherDate < end
                select new { l.AccountID, l.Debit, l.Credit, v.VoucherDate, Reference = o != null ? o.Reference : v.Reference, v.ID, v.Narration, v.VoucherNo, v.VoucherType, SourceType = o != null ? o.VoucherType : v.VoucherType };
        if (from.HasValue) q = q.Where(x => x.VoucherDate >= from.Value.Date);
        return (await q.ToListAsync())
            .Select(x => new PostedLine(x.AccountID, x.Debit, x.Credit, x.VoucherDate, x.Reference, x.ID, x.Narration, x.VoucherNo, x.VoucherType, x.SourceType))
            .ToList();
    }

    protected void Success(string text) { MessageClass = "success"; Message = text; }
    protected void Fail(string text) { MessageClass = "danger"; Message = text; }

    protected static string RootMessage(Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        return ex.Message;
    }

    protected Task PrintAsync(string elementId, string title) => JS.InvokeVoidAsync("hpm.printElement", elementId, title).AsTask();

    private void HandleLanguageChanged() => InvokeAsync(StateHasChanged);

    public virtual void Dispose()
    {
        Localizer.OnLanguageChanged -= HandleLanguageChanged;
        GC.SuppressFinalize(this);
    }
}

/// <summary>SourceType is the voucher type of the original voucher for a reversal (so a reversed expense still counts as an outflow).</summary>
public sealed record PostedLine(int AccountID, decimal Debit, decimal Credit, DateTime Date, string? Reference, long VoucherID, string? Narration, string VoucherNo, string VoucherType, string SourceType)
{
    public static bool IsOutflowType(string type) => type is "Expense" or "Payment" or "Purchase" or "Purchase Return";
}

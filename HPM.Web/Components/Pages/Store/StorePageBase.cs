using System.Security.Claims;
using HPM.Web.Data;
using HPM.Web.Localization;
using HPM.Web.Security;
using HPM.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace HPM.Web.Components.Pages.Store;

/// <summary>Common plumbing for the hospital store pages (lookups, current user, messages).</summary>
public abstract class StorePageBase : ComponentBase, IDisposable
{
    [Inject] protected IDbContextFactory<HpmDbContext> DbFactory { get; set; } = default!;
    [Inject] protected StoreInventoryService Inventory { get; set; } = default!;
    [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected NavigationManager Nav { get; set; } = default!;
    [Inject] protected I18nService Localizer { get; set; } = default!;

    protected StoreLookups Lookups { get; set; } = new();
    protected int CurrentUserId { get; private set; }
    protected bool IsManager { get; private set; }
    protected string Message { get; set; } = "";
    protected string MessageClass { get; set; } = "success";
    protected bool IsBusy { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Localizer.OnLanguageChanged += HandleLanguageChanged;
        var user = (await AuthStateProvider.GetAuthenticationStateAsync()).User;
        CurrentUserId = int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;
        IsManager = ModuleAccess.CanView(user, ModuleAccess.StoreManage);
        await ReloadLookupsAsync();
        await OnStoreInitializedAsync();
    }

    /// <summary>Page-specific initialisation after lookups and user are loaded.</summary>
    protected virtual Task OnStoreInitializedAsync() => Task.CompletedTask;

    protected async Task ReloadLookupsAsync(bool includeInactive = false)
    {
        await using var db = await DbFactory.CreateDbContextAsync();
        Lookups = await StoreLookups.LoadAsync(db, includeInactive);
    }

    protected async Task<Dictionary<int, string>> UserNamesAsync(IEnumerable<int> ids)
    {
        var list = ids.Where(x => x > 0).Distinct().ToList();
        if (list.Count == 0) return [];
        await using var db = await DbFactory.CreateDbContextAsync();
        var users = await db.Users.AsNoTracking().Where(u => list.Contains(u.Id)).ToListAsync();
        return users.ToDictionary(u => u.Id, u => u.GetEffectiveName());
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

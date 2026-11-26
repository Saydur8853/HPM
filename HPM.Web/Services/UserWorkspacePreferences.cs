using System.Collections.Concurrent;

namespace HPM.Web.Services;

/// <summary>
/// Server-side, per-user workspace choices (e.g. the pharmacy counter/store a cashier works on).
/// Values survive page refreshes, new tabs and navigation, and are wiped when the user signs out.
/// </summary>
public sealed class UserWorkspacePreferences
{
    public const string PharmacyStoreKey = "pharmacy.storeId";

    private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, string>> _store = new();

    public string? Get(int userId, string key)
        => userId > 0 && _store.TryGetValue(userId, out var prefs) && prefs.TryGetValue(key, out var value) ? value : null;

    public int? GetInt(int userId, string key)
        => int.TryParse(Get(userId, key), out var value) ? value : null;

    public void Set(int userId, string key, string? value)
    {
        if (userId <= 0) return;
        var prefs = _store.GetOrAdd(userId, _ => new ConcurrentDictionary<string, string>());
        if (string.IsNullOrWhiteSpace(value)) prefs.TryRemove(key, out _);
        else prefs[key] = value;
    }

    public void SetInt(int userId, string key, int value) => Set(userId, key, value.ToString());

    public void ClearUser(int userId) => _store.TryRemove(userId, out _);
}

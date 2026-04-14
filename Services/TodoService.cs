using ListImnida.Models;

namespace ListImnida.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoItem>> GetActiveItemsAsync();
    Task<IEnumerable<TodoItem>> GetCompletedItemsAsync();
    Task<TodoItem?> GetItemAsync(string id);
    Task<(bool Success, string Message)> AddItemAsync(TodoItem item);
    Task<(bool Success, string Message)> UpdateItemAsync(TodoItem item);
    Task<(bool Success, string Message)> CompleteItemAsync(TodoItem item);
    Task<(bool Success, string Message)> UncompleteItemAsync(TodoItem item);
    Task<(bool Success, string Message)> DeleteItemAsync(string id);
    Task<(bool Success, string Message, TodoItem? Item)> AddAndCompleteAsync(string title, string details);
}

public class TodoService : ITodoService
{
    private readonly ApiService _api;
    private readonly SessionService _session;

    // Local cache so GetItemAsync(id) can look up an item without a separate API call
    private readonly List<TodoItem> _cache = new();

    public TodoService(ApiService api, SessionService session)
    {
        _api = api;
        _session = session;
    }

    private int UserId => _session.CurrentUser?.Id ?? 0;

    public async Task<IEnumerable<TodoItem>> GetActiveItemsAsync()
    {
        var (_, _, items) = await _api.GetItemsAsync(UserId, "active");
        // Merge into cache
        foreach (var item in items)
            UpsertCache(item);
        return items;
    }

    public async Task<IEnumerable<TodoItem>> GetCompletedItemsAsync()
    {
        var (_, _, items) = await _api.GetItemsAsync(UserId, "inactive");
        foreach (var item in items)
            UpsertCache(item);
        return items;
    }

    public Task<TodoItem?> GetItemAsync(string id)
    {
        if (int.TryParse(id, out var intId))
            return Task.FromResult(_cache.FirstOrDefault(x => x.ItemId == intId));
        return Task.FromResult<TodoItem?>(null);
    }

    public async Task<(bool Success, string Message)> AddItemAsync(TodoItem item)
    {
        var (success, message, created) = await _api.AddItemAsync(item.Title, item.Details, UserId);
        if (success && created != null)
            UpsertCache(created);
        return (success, message);
    }

    public async Task<(bool Success, string Message)> UpdateItemAsync(TodoItem item)
    {
        var result = await _api.EditItemAsync(item.ItemId, item.Title, item.Details);
        if (result.Success)
            UpsertCache(item);
        return result;
    }

    public async Task<(bool Success, string Message)> CompleteItemAsync(TodoItem item)
    {
        var result = await _api.ChangeStatusAsync(item.ItemId, "inactive");
        if (result.Success)
        {
            item.IsCompleted = true;
            UpsertCache(item);
        }
        return result;
    }

    public async Task<(bool Success, string Message)> UncompleteItemAsync(TodoItem item)
    {
        var result = await _api.ChangeStatusAsync(item.ItemId, "active");
        if (result.Success)
        {
            item.IsCompleted = false;
            UpsertCache(item);
        }
        return result;
    }

    public async Task<(bool Success, string Message)> DeleteItemAsync(string id)
    {
        if (!int.TryParse(id, out var intId))
            return (false, "Invalid item id.");

        var result = await _api.DeleteItemAsync(intId);
        if (result.Success)
            _cache.RemoveAll(x => x.ItemId == intId);
        return result;
    }

    public async Task<(bool Success, string Message, TodoItem? Item)> AddAndCompleteAsync(string title, string details)
    {
        var (addOk, addMsg, item) = await _api.AddItemAsync(title, details, UserId);
        if (!addOk || item == null)
            return (false, addMsg, null);
        UpsertCache(item);

        var (completeOk, completeMsg) = await _api.ChangeStatusAsync(item.ItemId, "inactive");
        if (completeOk)
        {
            item.IsCompleted = true;
            UpsertCache(item);
        }
        return (completeOk, completeMsg, item);
    }

    // ── Cache helpers ─────────────────────────────────────────────────────────

    private void UpsertCache(TodoItem item)
    {
        var existing = _cache.FirstOrDefault(x => x.ItemId == item.ItemId);
        if (existing != null)
            _cache.Remove(existing);
        _cache.Add(item);
    }
}

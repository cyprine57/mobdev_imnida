using ListImnida.Models;

namespace ListImnida.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoItem>> GetItemsAsync();
    Task<TodoItem?> GetItemAsync(string id);
    Task AddItemAsync(TodoItem item);
    Task UpdateItemAsync(TodoItem item);
    Task DeleteItemAsync(string id);
}

public class TodoService : ITodoService
{
    private readonly List<TodoItem> _items = new();

    public TodoService()
    {
        // Add some mock data
        _items.Add(new TodoItem { Title = "Buy groceries", Details = "Milk, Eggs, Bread" });
        _items.Add(new TodoItem { Title = "Call mom", Details = "Catch up on weekend plans" });
        _items.Add(new TodoItem { Title = "Finish homework", Details = "Math assignment due tomorrow", IsCompleted = true });
    }

    public Task<IEnumerable<TodoItem>> GetItemsAsync()
    {
        return Task.FromResult<IEnumerable<TodoItem>>(_items);
    }

    public Task<TodoItem?> GetItemAsync(string id)
    {
        return Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    }

    public Task AddItemAsync(TodoItem item)
    {
        _items.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateItemAsync(TodoItem item)
    {
        var existingItem = _items.FirstOrDefault(x => x.Id == item.Id);
        if (existingItem != null)
        {
            existingItem.Title = item.Title;
            existingItem.Details = item.Details;
            existingItem.IsCompleted = item.IsCompleted;
        }
        return Task.CompletedTask;
    }

    public Task DeleteItemAsync(string id)
    {
        var existingItem = _items.FirstOrDefault(x => x.Id == id);
        if (existingItem != null)
        {
            _items.Remove(existingItem);
        }
        return Task.CompletedTask;
    }
}

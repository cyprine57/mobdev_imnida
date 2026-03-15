namespace ListImnida.Models;

public class TodoItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
}

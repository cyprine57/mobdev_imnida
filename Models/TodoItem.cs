namespace ListImnida.Models;

public class TodoItem
{
    // Local / API field mapping
    public int ItemId { get; set; }
    public string Id => ItemId.ToString(); // convenience alias used in navigation

    public string Title { get; set; } = string.Empty;    // maps to item_name
    public string Details { get; set; } = string.Empty;  // maps to item_description
    public bool IsCompleted { get; set; } = false;        // true when status == "inactive"
    public int UserId { get; set; }
    public string TimeModified { get; set; } = string.Empty;
}

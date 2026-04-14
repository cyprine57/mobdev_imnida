using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ListImnida.Models;

namespace ListImnida.Services;

// ─── Response wrappers ────────────────────────────────────────────────────────

public class ApiResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class SignInResponse : ApiResponse
{
    [JsonPropertyName("data")]
    public SignInData? Data { get; set; }
}

public class SignInData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fname")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lname")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("timemodified")]
    public string TimeModified { get; set; } = string.Empty;
}

public class AddItemResponse : ApiResponse
{
    [JsonPropertyName("data")]
    public TodoItemData? Data { get; set; }
}

public class GetItemsResponse : ApiResponse
{
    // The API returns items as a dictionary keyed by "0", "1", …
    [JsonPropertyName("data")]
    public Dictionary<string, TodoItemData>? Data { get; set; }

    [JsonPropertyName("count")]
    public string Count { get; set; } = "0";
}

public class TodoItemData
{
    [JsonPropertyName("item_id")]
    public int ItemId { get; set; }

    [JsonPropertyName("item_name")]
    public string ItemName { get; set; } = string.Empty;

    [JsonPropertyName("item_description")]
    public string ItemDescription { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = "active";

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("timemodified")]
    public string TimeModified { get; set; } = string.Empty;
}

// ─── Service ──────────────────────────────────────────────────────────────────

public class ApiService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://todo-list.dcism.org";

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    // ── Auth ──────────────────────────────────────────────────────────────────

    /// <summary>Sign in via GET query parameters.</summary>
    public async Task<(bool Success, string Message, User? User)> SignInAsync(string email, string password)
    {
        try
        {
            var url = $"{BaseUrl}/signin_action.php?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";
            var response = await _http.GetFromJsonAsync<SignInResponse>(url);

            if (response == null)
                return (false, "No response from server.", null);

            if (response.Status == 200 && response.Data != null)
            {
                var user = new User
                {
                    Id = response.Data.Id,
                    FirstName = response.Data.FirstName,
                    LastName = response.Data.LastName,
                    Email = response.Data.Email,
                    TimeModified = response.Data.TimeModified
                };
                return (true, response.Message, user);
            }

            return (false, response.Message, null);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}", null);
        }
    }

    /// <summary>Sign up via POST JSON body.</summary>
    public async Task<(bool Success, string Message)> SignUpAsync(
        string firstName, string lastName, string email, string password, string confirmPassword)
    {
        try
        {
            var body = new
            {
                first_name = firstName,
                last_name = lastName,
                email,
                password,
                confirm_password = confirmPassword
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResponse = await _http.PostAsync($"{BaseUrl}/signup_action.php", content);
            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>();

            if (response == null)
                return (false, "No response from server.");

            return (response.Status == 200, response.Message);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}");
        }
    }

    // ── Todo CRUD ─────────────────────────────────────────────────────────────

    /// <summary>Get active OR inactive items for a user.</summary>
    public async Task<(bool Success, string Message, List<TodoItem> Items)> GetItemsAsync(int userId, string status)
    {
        try
        {
            var url = $"{BaseUrl}/getItems_action.php?status={status}&user_id={userId}";
            var response = await _http.GetFromJsonAsync<GetItemsResponse>(url);

            if (response == null)
                return (false, "No response from server.", new List<TodoItem>());

            if (response.Status == 200 && response.Data != null)
            {
                var items = response.Data.Values.Select(d => MapToTodoItem(d)).ToList();
                return (true, response.Message, items);
            }

            // status 400 usually means no items / not found — return empty list, not an error
            return (true, response.Message, new List<TodoItem>());
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}", new List<TodoItem>());
        }
    }

    /// <summary>Add a new todo item.</summary>
    public async Task<(bool Success, string Message, TodoItem? Item)> AddItemAsync(
        string itemName, string itemDescription, int userId)
    {
        try
        {
            var body = new { item_name = itemName, item_description = itemDescription, user_id = userId };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResponse = await _http.PostAsync($"{BaseUrl}/addItem_action.php", content);
            var response = await httpResponse.Content.ReadFromJsonAsync<AddItemResponse>();

            if (response == null)
                return (false, "No response from server.", null);

            if (response.Status == 200 && response.Data != null)
                return (true, response.Message, MapToTodoItem(response.Data));

            return (false, response.Message, null);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}", null);
        }
    }

    /// <summary>Edit an existing todo item (name + description).</summary>
    public async Task<(bool Success, string Message)> EditItemAsync(
        int itemId, string itemName, string itemDescription)
    {
        try
        {
            var body = new { item_name = itemName, item_description = itemDescription, item_id = itemId };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}/editItem_action.php")
            {
                Content = content
            };
            var httpResponse = await _http.SendAsync(request);
            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>();

            if (response == null)
                return (false, "No response from server.");

            return (response.Status == 200, response.Message);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}");
        }
    }

    /// <summary>Change the status of a todo item (active / inactive).</summary>
    public async Task<(bool Success, string Message)> ChangeStatusAsync(int itemId, string status)
    {
        try
        {
            var body = new { status, item_id = itemId };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}/statusItem_action.php")
            {
                Content = content
            };
            var httpResponse = await _http.SendAsync(request);
            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>();

            if (response == null)
                return (false, "No response from server.");

            return (response.Status == 200, response.Message);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}");
        }
    }

    /// <summary>Delete a todo item by item_id.</summary>
    public async Task<(bool Success, string Message)> DeleteItemAsync(int itemId)
    {
        try
        {
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{BaseUrl}/deleteItem_action.php?item_id={itemId}");
            var httpResponse = await _http.SendAsync(request);
            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>();

            if (response == null)
                return (false, "No response from server.");

            return (response.Status == 200, response.Message);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}");
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static TodoItem MapToTodoItem(TodoItemData d) => new TodoItem
    {
        ItemId = d.ItemId,
        Title = d.ItemName,
        Details = d.ItemDescription,
        IsCompleted = d.Status == "inactive",
        UserId = d.UserId,
        TimeModified = d.TimeModified
    };
}

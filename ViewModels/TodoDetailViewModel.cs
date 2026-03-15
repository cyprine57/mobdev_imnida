using System.Windows.Input;
using ListImnida.Models;
using ListImnida.Services;

namespace ListImnida.ViewModels;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public class TodoDetailViewModel : BaseViewModel
{
    private readonly ITodoService _todoService;

    private string _itemId = string.Empty;
    public string ItemId
    {
        get => _itemId;
        set
        {
            _itemId = value;
            LoadItemId(value);
        }
    }

    private string _itemTitle = string.Empty;
    public string ItemTitle
    {
        get => _itemTitle;
        set => SetProperty(ref _itemTitle, value);
    }

    private string _itemDetails = string.Empty;
    public string ItemDetails
    {
        get => _itemDetails;
        set => SetProperty(ref _itemDetails, value);
    }
    
    // To decide whether we are creating a new item or updating an existing one
    public bool IsNewItem => string.IsNullOrWhiteSpace(ItemId);

    public ICommand SaveCommand { get; }
    public ICommand CompleteCommand { get; }
    public ICommand DeleteCommand { get; }

    public TodoDetailViewModel(ITodoService todoService)
    {
        _todoService = todoService;
        
        SaveCommand = new Command(async () => await OnSave());
        CompleteCommand = new Command(async () => await OnComplete());
        DeleteCommand = new Command(async () => await OnDelete());
    }

    private async void LoadItemId(string itemId)
    {
        Title = "Edit Todo";
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Title = "New Todo";
            return;
        }

        try
        {
            var item = await _todoService.GetItemAsync(itemId);
            if (item != null)
            {
                ItemTitle = item.Title;
                ItemDetails = item.Details;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load item. {ex.Message}");
        }
    }

    private async Task OnSave()
    {
        if (string.IsNullOrWhiteSpace(ItemTitle))
        {
            await Application.Current!.MainPage!.DisplayAlert("Validation", "Title is required.", "OK");
            return;
        }

        if (IsNewItem)
        {
            var newItem = new TodoItem
            {
                Title = ItemTitle,
                Details = ItemDetails,
                IsCompleted = false
            };
            await _todoService.AddItemAsync(newItem);
        }
        else
        {
            var item = await _todoService.GetItemAsync(ItemId);
            if (item != null)
            {
                item.Title = ItemTitle;
                item.Details = ItemDetails;
                await _todoService.UpdateItemAsync(item);
            }
        }

        await Shell.Current.GoToAsync("..");
    }

    private async Task OnComplete()
    {
        if (IsNewItem)
        {
            var newItem = new TodoItem
            {
                Title = ItemTitle,
                Details = ItemDetails,
                IsCompleted = true
            };
            await _todoService.AddItemAsync(newItem);
        }
        else
        {
            var item = await _todoService.GetItemAsync(ItemId);
            if (item != null)
            {
                item.IsCompleted = true;
                item.Title = ItemTitle;
                item.Details = ItemDetails;
                await _todoService.UpdateItemAsync(item);
            }
        }
        
        await Shell.Current.GoToAsync("..");
    }

    private async Task OnDelete()
    {
        if (!IsNewItem)
        {
            await _todoService.DeleteItemAsync(ItemId);
        }
        
        await Shell.Current.GoToAsync("..");
    }
}

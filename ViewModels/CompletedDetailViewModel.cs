using System.Windows.Input;
using ListImnida.Models;
using ListImnida.Services;

namespace ListImnida.ViewModels;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public class CompletedDetailViewModel : BaseViewModel
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

    public ICommand UpdateCommand { get; }
    public ICommand IncompleteCommand { get; }
    public ICommand DeleteCommand { get; }

    public CompletedDetailViewModel(ITodoService todoService)
    {
        _todoService = todoService;
        
        UpdateCommand = new Command(async () => await OnUpdate());
        IncompleteCommand = new Command(async () => await OnIncomplete());
        DeleteCommand = new Command(async () => await OnDelete());
    }

    private async void LoadItemId(string itemId)
    {
        Title = "Completed Todo";
        if (string.IsNullOrWhiteSpace(itemId))
            return;

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

    private async Task OnUpdate()
    {
        if (string.IsNullOrWhiteSpace(ItemTitle))
        {
            await Application.Current!.MainPage!.DisplayAlert("Validation", "Title is required.", "OK");
            return;
        }

        var item = await _todoService.GetItemAsync(ItemId);
        if (item != null)
        {
            item.Title = ItemTitle;
            item.Details = ItemDetails;
            await _todoService.UpdateItemAsync(item);
        }

        await Shell.Current.GoToAsync("..");
    }

    private async Task OnIncomplete()
    {
        var item = await _todoService.GetItemAsync(ItemId);
        if (item != null)
        {
            item.IsCompleted = false;
            // Also apply potential edits while making it incomplete
            item.Title = ItemTitle;
            item.Details = ItemDetails;
            
            await _todoService.UpdateItemAsync(item);
        }
        
        await Shell.Current.GoToAsync("..");
    }

    private async Task OnDelete()
    {
        await _todoService.DeleteItemAsync(ItemId);
        await Shell.Current.GoToAsync("..");
    }
}

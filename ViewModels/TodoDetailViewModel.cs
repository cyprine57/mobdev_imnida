using System.Windows.Input;
using ListImnida.Models;
using ListImnida.Services;

namespace ListImnida.ViewModels;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public class TodoDetailViewModel : BaseViewModel
{
    private readonly ITodoService _todoService;

    // ItemId comes as a string from the query parameter
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

    public bool IsNewItem => string.IsNullOrWhiteSpace(ItemId) || ItemId == "0";

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
        if (string.IsNullOrWhiteSpace(itemId) || itemId == "0")
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

        IsBusy = true;
        try
        {
            bool success;
            string message;

            if (IsNewItem)
            {
                (success, message) = await _todoService.AddItemAsync(new TodoItem
                {
                    Title = ItemTitle,
                    Details = ItemDetails
                });
            }
            else
            {
                var item = await _todoService.GetItemAsync(ItemId);
                if (item != null)
                {
                    item.Title = ItemTitle;
                    item.Details = ItemDetails;
                    (success, message) = await _todoService.UpdateItemAsync(item);
                }
                else
                {
                    (success, message) = (false, "Item not found.");
                }
            }

            if (!success)
                await Application.Current!.MainPage!.DisplayAlert("Error", message, "OK");
        }
        finally
        {
            IsBusy = false;
        }

        await Shell.Current.GoToAsync("..");
    }

    private async Task OnComplete()
    {
        if (string.IsNullOrWhiteSpace(ItemTitle))
        {
            await Application.Current!.MainPage!.DisplayAlert("Validation", "Title is required.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            if (IsNewItem)
            {
                // Add first, then mark complete
                var (addOk, addMsg, _) = await _todoService.AddAndCompleteAsync(ItemTitle, ItemDetails);

                if (!addOk)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", addMsg, "OK");
                    return;
                }
            }
            else
            {
                var item = await _todoService.GetItemAsync(ItemId);
                if (item != null)
                {
                    item.Title = ItemTitle;
                    item.Details = ItemDetails;
                    // Save edits first, then mark complete
                    await _todoService.UpdateItemAsync(item);
                    await _todoService.CompleteItemAsync(item);
                }
            }
        }
        finally
        {
            IsBusy = false;
        }

        await Shell.Current.GoToAsync("..");
    }

    private async Task OnDelete()
    {
        if (!IsNewItem)
        {
            IsBusy = true;
            try
            {
                var (success, message) = await _todoService.DeleteItemAsync(ItemId);
                if (!success)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", message, "OK");
                    return;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        await Shell.Current.GoToAsync("..");
    }
}

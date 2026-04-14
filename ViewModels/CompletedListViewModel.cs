using System.Collections.ObjectModel;
using System.Windows.Input;
using ListImnida.Models;
using ListImnida.Services;

namespace ListImnida.ViewModels;

public class CompletedListViewModel : BaseViewModel
{
    private readonly ITodoService _todoService;

    public ObservableCollection<TodoItem> Items { get; } = new();

    private TodoItem? _selectedItem;
    public TodoItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            SetProperty(ref _selectedItem, value);
            if (value != null)
                OnItemSelected(value);
        }
    }

    public ICommand LoadItemsCommand { get; }

    public CompletedListViewModel(ITodoService todoService)
    {
        Title = "Completed";
        _todoService = todoService;
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
    }

    public async Task ExecuteLoadItemsCommand()
    {
        IsBusy = true;
        try
        {
            Items.Clear();
            var items = await _todoService.GetCompletedItemsAsync();
            foreach (var item in items)
                Items.Add(item);
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnItemSelected(TodoItem item)
    {
        if (item == null) return;

        await Shell.Current.GoToAsync($"{nameof(Views.CompletedDetailPage)}?ItemId={item.ItemId}");
        SelectedItem = null;
    }
}

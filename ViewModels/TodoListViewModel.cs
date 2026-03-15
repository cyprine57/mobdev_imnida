using System.Collections.ObjectModel;
using System.Windows.Input;
using ListImnida.Models;
using ListImnida.Services;

namespace ListImnida.ViewModels;

public class TodoListViewModel : BaseViewModel
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
            {
                OnItemSelected(value);
            }
        }
    }

    public ICommand LoadItemsCommand { get; }
    public ICommand AddItemCommand { get; }

    public TodoListViewModel(ITodoService todoService)
    {
        Title = "To Do";
        _todoService = todoService;
        
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        AddItemCommand = new Command(async () => await OnAddItem());
    }

    public async Task ExecuteLoadItemsCommand()
    {
        IsBusy = true;

        try
        {
            Items.Clear();
            var items = await _todoService.GetItemsAsync();
            foreach (var item in items.Where(i => !i.IsCompleted))
            {
                Items.Add(item);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnAddItem()
    {
        // Pass null or a new item to indicate creation
        await Shell.Current.GoToAsync(nameof(Views.TodoDetailPage));
    }

    private async void OnItemSelected(TodoItem item)
    {
        if (item == null)
            return;

        // Navigate to the detail page and pass the ID
        await Shell.Current.GoToAsync($"{nameof(Views.TodoDetailPage)}?ItemId={item.Id}");

        // Clear selection
        SelectedItem = null;
    }
}

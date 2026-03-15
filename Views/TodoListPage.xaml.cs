using ListImnida.ViewModels;

namespace ListImnida.Views;

public partial class TodoListPage : ContentPage
{
    private readonly TodoListViewModel _viewModel;

    public TodoListPage(TodoListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ExecuteLoadItemsCommand().FireAndForgetSafeAsync();
    }
}

public static class TaskExtensions
{
    public static async void FireAndForgetSafeAsync(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            // Log exception
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }
}

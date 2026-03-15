using ListImnida.ViewModels;

namespace ListImnida.Views;

public partial class CompletedListPage : ContentPage
{
    private readonly CompletedListViewModel _viewModel;

    public CompletedListPage(CompletedListViewModel viewModel)
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

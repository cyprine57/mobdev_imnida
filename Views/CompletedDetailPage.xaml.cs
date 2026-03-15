using ListImnida.ViewModels;

namespace ListImnida.Views;

public partial class CompletedDetailPage : ContentPage
{
    public CompletedDetailPage(CompletedDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

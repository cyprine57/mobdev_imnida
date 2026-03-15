using ListImnida.ViewModels;

namespace ListImnida.Views;

public partial class TodoDetailPage : ContentPage
{
    public TodoDetailPage(TodoDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

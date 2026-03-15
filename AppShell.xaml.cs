namespace ListImnida;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for pages that are not tabs
        Routing.RegisterRoute(nameof(Views.SignUpPage), typeof(Views.SignUpPage));
        Routing.RegisterRoute(nameof(Views.TodoDetailPage), typeof(Views.TodoDetailPage));
        Routing.RegisterRoute(nameof(Views.CompletedDetailPage), typeof(Views.CompletedDetailPage));
    }
}

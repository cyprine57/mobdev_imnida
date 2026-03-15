namespace ListImnida;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Start with the Login Page. It will navigate to AppShell upon successful login.
        MainPage = new Views.LoginPage(new ViewModels.LoginViewModel());
    }
}

using System.Windows.Input;

namespace ListImnida.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    // Simulate a logged-in user (in a real app this would come from a session/auth service)
    private string _userName = "Demo User";
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    private string _email = "demo@listimnida.com";
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public ICommand SignOutCommand { get; }

    public ProfileViewModel()
    {
        Title = "Profile";
        SignOutCommand = new Command(OnSignOut);
    }

    private void OnSignOut()
    {
        // Clear any session & go back to Login Page
        Application.Current!.MainPage = new Views.LoginPage(new LoginViewModel());
    }
}

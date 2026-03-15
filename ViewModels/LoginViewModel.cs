using System.Windows.Input;

namespace ListImnida.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand GoToSignUpCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(OnLoginClicked);
        GoToSignUpCommand = new Command(OnGoToSignUpClicked);
    }

    private void OnLoginClicked(object obj)
    {
        // Simple logic: navigate to AppShell on login
        Application.Current!.MainPage = new AppShell();
    }

    private async void OnGoToSignUpClicked(object obj)
    {
        // We will just swap the main page for simplicity 
        Application.Current!.MainPage = new Views.SignUpPage(new SignUpViewModel());
    }
}

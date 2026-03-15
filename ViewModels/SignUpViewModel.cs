using System.Windows.Input;

namespace ListImnida.ViewModels;

public class SignUpViewModel : BaseViewModel
{
    private string _userName = string.Empty;
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

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

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public ICommand SignUpCommand { get; }
    public ICommand GoBackCommand { get; }

    public SignUpViewModel()
    {
        SignUpCommand = new Command(OnSignUpClicked);
        GoBackCommand = new Command(OnGoBackClicked);
    }

    private void OnSignUpClicked(object obj)
    {
        // Add basic validation for demonstration based on requirements
        if (Password != ConfirmPassword)
        {
            Application.Current?.MainPage?.DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        // Navigate to AppShell upon successful sign up
        Application.Current!.MainPage = new AppShell();
    }

    private void OnGoBackClicked(object obj)
    {
        // Go back to Login Page
        Application.Current!.MainPage = new Views.LoginPage(new LoginViewModel());
    }
}

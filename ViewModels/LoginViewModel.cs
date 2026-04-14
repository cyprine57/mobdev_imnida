using System.Windows.Input;
using ListImnida.Services;

namespace ListImnida.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly ApiService _api;
    private readonly SessionService _session;

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

    public LoginViewModel(ApiService api, SessionService session)
    {
        _api = api;
        _session = session;

        LoginCommand = new Command(async () => await OnLoginClicked());
        GoToSignUpCommand = new Command(OnGoToSignUpClicked);
    }

    private async Task OnLoginClicked()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current!.MainPage!.DisplayAlert("Validation", "Email and password are required.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var (success, message, user) = await _api.SignInAsync(Email, Password);

            if (success && user != null)
            {
                _session.SetUser(user);
                Application.Current!.MainPage = new AppShell();
            }
            else
            {
                await Application.Current!.MainPage!.DisplayAlert("Sign In Failed", message, "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnGoToSignUpClicked(object obj)
    {
        // Navigate to Sign Up page — resolving through DI via service locator pattern
        var signUpVm = IPlatformApplication.Current!.Services.GetRequiredService<SignUpViewModel>();
        Application.Current!.MainPage = new NavigationPage(new Views.SignUpPage(signUpVm));
    }
}

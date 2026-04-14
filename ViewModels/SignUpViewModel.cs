using System.Windows.Input;
using ListImnida.Services;

namespace ListImnida.ViewModels;

public class SignUpViewModel : BaseViewModel
{
    private readonly ApiService _api;

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
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

    public SignUpViewModel(ApiService api)
    {
        _api = api;
        SignUpCommand = new Command(async () => await OnSignUpClicked());
        GoBackCommand = new Command(OnGoBackClicked);
    }

    private async Task OnSignUpClicked()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current!.MainPage!.DisplayAlert("Validation", "All fields are required.", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Application.Current!.MainPage!.DisplayAlert("Validation", "Passwords do not match.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var (success, message) = await _api.SignUpAsync(FirstName, LastName, Email, Password, ConfirmPassword);

            if (success)
            {
                await Application.Current!.MainPage!.DisplayAlert("Success", message, "OK");
                // Go back to login after successful sign-up
                var loginVm = IPlatformApplication.Current!.Services.GetRequiredService<LoginViewModel>();
                Application.Current!.MainPage = new NavigationPage(new Views.LoginPage(loginVm));
            }
            else
            {
                await Application.Current!.MainPage!.DisplayAlert("Sign Up Failed", message, "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnGoBackClicked(object obj)
    {
        var loginVm = IPlatformApplication.Current!.Services.GetRequiredService<LoginViewModel>();
        Application.Current!.MainPage = new NavigationPage(new Views.LoginPage(loginVm));
    }
}

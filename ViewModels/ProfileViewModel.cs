using System.Windows.Input;
using ListImnida.Services;

namespace ListImnida.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private readonly SessionService _session;

    private string _fullName = string.Empty;
    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public ICommand SignOutCommand { get; }

    public ProfileViewModel(SessionService session)
    {
        Title = "Profile";
        _session = session;
        SignOutCommand = new Command(OnSignOut);

        if (_session.CurrentUser != null)
        {
            FullName = _session.CurrentUser.FullName;
            Email = _session.CurrentUser.Email;
        }
    }

    private void OnSignOut()
    {
        _session.ClearUser();
        var loginVm = IPlatformApplication.Current!.Services.GetRequiredService<LoginViewModel>();
        Application.Current!.MainPage = new Views.LoginPage(loginVm);
    }
}

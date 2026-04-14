using ListImnida.Models;

namespace ListImnida.Services;

/// <summary>
/// Holds the currently authenticated user for the lifetime of the app session.
/// Register as a Singleton so the same instance is shared everywhere.
/// </summary>
public class SessionService
{
    public User? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser != null;

    public void SetUser(User user)
    {
        CurrentUser = user;
    }

    public void ClearUser()
    {
        CurrentUser = null;
    }
}

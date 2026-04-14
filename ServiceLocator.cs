namespace ListImnida;

/// <summary>
/// Simple service locator that holds the root IServiceProvider
/// set by MauiProgram after the app is built.
/// </summary>
public static class ServiceLocator
{
    private static IServiceProvider? _provider;

    public static void SetProvider(IServiceProvider provider)
    {
        _provider = provider;
    }

    public static T Get<T>() where T : notnull
    {
        if (_provider == null)
            throw new InvalidOperationException("ServiceLocator has not been initialized.");
        return _provider.GetRequiredService<T>();
    }
}

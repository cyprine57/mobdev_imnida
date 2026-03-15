using Microsoft.Extensions.Logging;
using ListImnida.Services;
using ListImnida.ViewModels;
using ListImnida.Views;

namespace ListImnida;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services
        builder.Services.AddSingleton<ITodoService, TodoService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<SignUpViewModel>();
        builder.Services.AddTransient<TodoListViewModel>();
        builder.Services.AddTransient<TodoDetailViewModel>();
        builder.Services.AddTransient<CompletedListViewModel>();
        builder.Services.AddTransient<CompletedDetailViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Register Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SignUpPage>();
        builder.Services.AddTransient<TodoListPage>();
        builder.Services.AddTransient<TodoDetailPage>();
        builder.Services.AddTransient<CompletedListPage>();
        builder.Services.AddTransient<CompletedDetailPage>();
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

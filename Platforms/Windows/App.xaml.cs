using Microsoft.UI.Xaml;

namespace ListImnida.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
        this.UnhandledException += OnUnhandledException;
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // Write to a crash log file so we can see the error
        var logPath = System.IO.Path.Combine(
            System.AppContext.BaseDirectory, "crash.log");
        System.IO.File.AppendAllText(logPath,
            $"[{System.DateTime.Now}] {e.Exception}\n{e.Exception?.StackTrace}\n\n");
        e.Handled = true;
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

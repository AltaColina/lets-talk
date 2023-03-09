using Microsoft.Extensions.Configuration;

namespace LetsTalk;

public partial class App : Application
{
    public App(AppShell appShell)
    {
        InitializeComponent();

        MainPage = appShell;
    }
}

using Microsoft.Extensions.Configuration;

namespace LetsTalk.App;

public partial class App : Application
{
    public static new App Current { get => (App)Application.Current!; }
    public IServiceProvider Services { get; }
    public IConfiguration Configuration { get; }

    public App(IServiceProvider services, IConfiguration configuration)
    {
        InitializeComponent();

        MainPage = new AppShell();
        Services = services;
        Configuration = configuration;
    }
}

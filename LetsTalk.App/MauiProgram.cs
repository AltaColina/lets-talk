namespace LetsTalk.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        ConfigureRoutes();
        ConfigureServices(builder.Services);



        return builder.Build();
    }

    private static void ConfigureRoutes()
    {
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(ChatPage), typeof(ChatPage));
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ILetsTalkHubClient, LetsTalkHubClient>();
        services.AddLetsTalkHttpClient(opts => opts.BaseAddress = new Uri("https://localhost:7219/letsTalk"));
        
        services.AddSingleton<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ChatViewModel>();
        
        services.AddSingleton<MainPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<ChatPage>();
    }
}

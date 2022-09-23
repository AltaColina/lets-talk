using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;

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
        builder.Configuration.AddContainersConfiguration("/LetsTalk");
        AddServices(builder.Services, builder.Configuration);
        AddViewModels(builder.Services);
        AddPages(builder.Services);
        
        return builder.Build();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        services.AddLetsTalkSettings(configuration);
        services.AddLetsTalkHttpClient(configuration);
        services.AddLetsTalkHubClient(configuration);
    }

    private static void AddViewModels(IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ChatViewModel>();
    }

    private static void AddPages(IServiceCollection services)
    {
        services.AddSingleton<MainPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<ChatPage>();
    }
}

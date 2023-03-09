using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;

namespace LetsTalk.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {

#if ANDROID
        const string host = "10.0.2.2";
#else
        const string host = "localhost";
#endif
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Configuration.AddContainersConfiguration(host, "/LetsTalk");
        AddServices(builder.Services, builder.Configuration);
        AddViewModels(builder.Services);
        AddPages(builder.Services);

        return builder.Build();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<RoomConnectionManager>();
        services.AddLetsTalkSettings();
        services.AddLetsTalkHttpClient(configuration);
        services.AddLetsTalkHubClient();
    }

    private static void AddViewModels(IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RoomViewModel>();
        services.AddTransient<AddRoomViewModel>();
    }

    private static void AddPages(IServiceCollection services)
    {
        services.AddSingleton<AppShell>();
        services.AddSingleton<MainPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<RoomPage>();
        services.AddTransient<AddRoomPage>();
    }
}

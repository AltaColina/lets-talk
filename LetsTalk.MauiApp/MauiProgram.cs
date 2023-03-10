using Microsoft.Extensions.Configuration;

namespace LetsTalk;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {

#if ANDROID
        const string hostname = "10.0.2.2";
#else
        const string hostname = "localhost";
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
        builder.Configuration.AddContainersConfiguration(hostname);
        AddServices(builder.Services, builder.Configuration);
        AddViewModels(builder.Services);
        AddPages(builder.Services);

        return builder.Build();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddLetsTalk(configuration);
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<RoomConnectionManager>();
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

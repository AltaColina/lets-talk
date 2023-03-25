using LetsTalk;
using LetsTalk.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configuration => configuration.AddContainersConfiguration("localhost"))
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplication();
        services.AddLetsTalk(hostContext.Configuration);
        services.AddTransient<MessageRecipient>();
        services.AddHttpClient("Identity", (services, client) => client.BaseAddress = new Uri(services.GetRequiredService<IConfiguration>().GetConnectionString("LetsTalk.Identity")!));
        services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();
        services.AddLetsTalk(hostContext.Configuration);
        services.AddSingleton<App>();
    })
    .UseConsoleLifetime()
    .Build();

host.Start();

var app = host.Services.GetRequiredService<App>();

await app.LoginAsync();

Console.WriteLine(await app.PingWebApi());

await app.ConnectToHubAsync();

bool joinAnotherRoom = false;
do
{
    await app.JoinChatRoomAsync();

    await app.SendAndReceiveMessagesAsync();

    Console.Write("Join another room? (Y/N): ");
    joinAnotherRoom = StringComparer.InvariantCultureIgnoreCase.Equals("Y", Console.ReadLine());
}
while (joinAnotherRoom);

await app.DisconnectFromHubAsync();

await host.WaitForShutdownAsync();

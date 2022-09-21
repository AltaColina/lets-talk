using LetsTalk.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using LetsTalk.Interfaces;
using LetsTalk;
using Microsoft.Extensions.Configuration;
using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Console;
using Docker.DotNet;
using Docker.DotNet.Models;
using LetsTalk.Dtos.Users;
using LetsTalk.Dtos.Auths;
using LetsTalk.Dtos.Chats;

var dockerClient = new DockerClientConfiguration().CreateClient();
var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true });
var container = containers.Single(c => c.Names.Contains("/LetsTalk"));
var port = container.Ports.First();
var address = $"https://localhost:{port.PublicPort}";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(hostContext => hostContext.AddInMemoryCollection(new Dictionary<string, string>
    {
        ["LetsTalkRestAddress"] = address,
        ["LetsTalkHubAddress"] = $"{address}/letstalk",
    }))
    .ConfigureServices((hostContext, services) => services
        .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
        .AddLetsTalkSettings(hostContext.Configuration)
        .AddLetsTalkHttpClient(hostContext.Configuration)
        .AddLetsTalkHubClient(hostContext.Configuration))
    .UseConsoleLifetime()
    .Build();

host.Start();


var startChoice = -2;
while (startChoice < 0 || startChoice > 2)
{
    Console.WriteLine("1: Register");
    Console.WriteLine("2: Login");
    Console.WriteLine("0: Exit");
    var line = Console.ReadLine();
    if (!Int32.TryParse(line, out startChoice))
        Console.WriteLine("Invalid choice.");
}

if (startChoice == 0)
    return;

var httpClient = host.Services.GetRequiredService<ILetsTalkHttpClient>();

var settings = host.Services.GetRequiredService<ILetsTalkSettings>();
// Register or login.
if (startChoice == 1)
{
    while (!settings.IsAuthenticated)
    {
        Console.Write("Username: ");
        var username = Console.ReadLine()!;
        Console.Write("Password: ");
        var password = Console.ReadLine()!;
        try
        {
            settings.Authentication = await httpClient.RegisterAsync(new RegisterRequest { Username = username, Password = password });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
else
{
    // Login.
    while (!settings.IsAuthenticated)
    {
        Console.Write("Username: ");
        var username = Console.ReadLine()!;
        Console.Write("Password: ");
        var password = Console.ReadLine()!;
        try
        {
            settings.Authentication = await httpClient.LoginAsync(new LoginRequest { Username = username, Password = password });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
var hubClient = host.Services.GetRequiredService<ILetsTalkHubClient>();
await hubClient.ConnectAsync();

var chat = default(ChatDto);
while (chat is null)
{
    var chats = (await httpClient.GetChatsAsync(settings.Authentication.AccessToken.Id)).Chats;
    Console.WriteLine("Select a channel to join by typing its number.");
    for (int i = 0; i < chats.Count; ++i)
        Console.WriteLine($"{i + 1}: {chats[i].Name}");
    Console.WriteLine("0: Exit LetsTalk");
    var input = Console.ReadLine();
    if (!Int32.TryParse(input, out int number))
        Console.WriteLine("Input must be a number.");
    else if (number == 0)
        break;
    else if ((number - 1) is var index && index >= chats.Count)
        Console.WriteLine($"Must be a number between {0} and {chats.Count}.");
    else
        chat = chats[index];
}


if (chat is not null)
{
    var messenger = host.Services.GetRequiredService<IMessenger>();
    var recipient = new MessageRecipient();
    messenger.Register(recipient);

    await hubClient.JoinChatAsync(chat.Id);

    // Send messages.
    while (Console.ReadLine() is string message && message != "exit")
    {
        var (left, top) = Console.GetCursorPosition();
        Console.SetCursorPosition(left, top - 1);
        await hubClient.SendChatMessageAsync(chat.Id, message);
    }

    await hubClient.LeaveChatAsync(chat.Id);
    await hubClient.DisconnectAsync();
}

await host.WaitForShutdownAsync();

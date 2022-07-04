using LetsTalk.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using LetsTask.Console;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) => services.AddHttpClient<LetsTalkHttpClient>("LetsTalk", http => http.BaseAddress = new("https://localhost:7219/letsTalk")))
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

var httpClient = host.Services.GetRequiredService<LetsTalkHttpClient>();

var person = default(Person)!;
var accessToken = default(Token)!;
var refreshToken = default(Token)!;
// Register or login.
if (startChoice == 1)
{
    while (person is null)
    {
        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();
        try
        {
            var response = await httpClient.RegisterAsync(new RegisterRequest { Username = username, Password = password });
            person = response.Person;
            accessToken = response.AccessToken;
            refreshToken = response.RefreshToken;
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
    while (accessToken is null)
    {
        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();
        try
        {
            var response = await httpClient.LoginAsync(new LoginRequest { Username = username, Password = password });
            person = response.Person;
            accessToken = response.AccessToken;
            refreshToken = response.RefreshToken;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
// Add Authorization to HttpClient.
httpClient.AcessToken = accessToken;

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7219/letsTalk", opts => opts.AccessTokenProvider = () => Task.FromResult<string?>(accessToken.Id))
    .Build();

connection.On<Message>(Methods.ServerMessage, message => Console.WriteLine(message.Content));
connection.On<Message>(Methods.UserMessage, message => Console.WriteLine($"{message.Username}: {message.Content}"));

await connection.StartAsync();

var chat = default(Chat);
while (chat is null)
{
    var chats = (await httpClient.ChatGetAsync(new ChatGetRequest())).Chats;
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
    await connection.InvokeAsync(Methods.Join, chat.Id);

    // Send messages.
    while (Console.ReadLine() is string message && message != "exit")
        await connection.InvokeAsync(Methods.UserMessage, chat.Id, message);

    await connection.InvokeAsync(Methods.Leave, chat.Id);
}

await host.WaitForShutdownAsync();

using LetsTalk;
using LetsTalk.Rooms;
using LetsTalk.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Mime;
using System.Reflection;
using System.Text;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configuration => configuration.AddContainersConfiguration("localhost"))
    .ConfigureServices((hostContext, services) => services
        .AddApplication()
        .AddLetsTalk(hostContext.Configuration)
        .AddTransient<MessageRecipient>())
    .UseConsoleLifetime()
    .Build();

host.Start();

var httpClient = host.Services.GetRequiredService<ILetsTalkHttpClient>();

var settings = host.Services.GetRequiredService<ILetsTalkSettings>();

while (!settings.IsAuthenticated)
{
    var startChoice = -1;

    Console.WriteLine("1: Register");
    Console.WriteLine("2: Login");
    Console.WriteLine("0: Exit");
    var line = Console.ReadLine();

    // Invalid
    if (!Int32.TryParse(line, out startChoice) || startChoice is < 0 or > 2)
    {
        Console.WriteLine($"Invalid choice '{line}'");
        continue;
    }

    // Exit
    if (startChoice == 0)
    {
        return;
    }

    Console.Write("Username: ");
    var username = Console.ReadLine()!;
    Console.Write("Password: ");
    var password = Console.ReadLine()!;

    try
    {
        settings.Authentication = startChoice is 1
            ? await httpClient.RegisterAsync(username, username)
            : await httpClient.LoginAsync(username, username);
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine(ex);
    }
}

var hubClient = host.Services.GetRequiredService<ILetsTalkHubClient>();
await hubClient.ConnectAsync();

var rooms = (await hubClient.GetUserRoomsAsync()).Rooms;
var room = default(RoomDto);
while (room is null)
{
    Console.WriteLine("Select a channel to listen to by typing its number.");
    int i = 0;
    for (; i < rooms.Count; ++i)
        Console.WriteLine($"{i + 1}: {rooms[i].Name}");
    Console.WriteLine($"{i + 1}: Join another channel.");
    Console.WriteLine("0: Exit LetsTalk");
    var input = Console.ReadLine();
    if (!Int32.TryParse(input, out int number))
    {
        Console.WriteLine("Input must be a number.");
    }
    else if (number < 0 || number > rooms.Count + 1)
    {
        Console.WriteLine($"Must be a number between {0} and {rooms.Count + 1}.");
    }
    else if (number == 0)
    {
        break;
    }
    else if (number == rooms.Count + 1)
    {
        var allRooms = (await hubClient.GetUserAvailableRoomsAsync()).Rooms;
        allRooms.RemoveAll(room => settings.Authentication.User.Rooms.Contains(room.Id));
        Console.WriteLine("Available rooms:");
        foreach (var item in allRooms)
            Console.WriteLine($"- {item.Name} (id: {item.Id})");
        Console.WriteLine("Select room to join. Type '/back' to go return to previous menu.");
        Console.Write("Room Id: ");
        var roomId = Console.ReadLine()!;
        if (roomId != "/back")
        {
            var newRoom = await httpClient.GetRoomAsync(roomId, settings.Authentication.AccessToken);
            rooms.Add(newRoom);
            await hubClient.JoinRoomAsync(roomId);
        }
    }
    else
    {
        room = rooms[number - 1];
    }
}

static void Recipient_MessageReceived(object? sender, string message) => Console.WriteLine(message);

var commandToContentTypeMap = typeof(LetsTalk.Messaging.MimeType.Image)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .ToDictionary(f => $"/{f.Name}", f => (string)f.GetValue(null)!, StringComparer.InvariantCultureIgnoreCase);

if (room is not null)
{
    var recipient = host.Services.GetRequiredService<MessageRecipient>();
    recipient.ListenToRoom(room.Id);
    recipient.MessageReceived += Recipient_MessageReceived;

    var users = (await hubClient.GetLoggedRoomUsersAsync(room.Id)).Users;
    users.RemoveAll(user => user.Id == settings.Authentication.User.Id);
    Console.WriteLine($"Listening to room {room.Name} (id: {room.Id}). Logged users:");
    foreach (var item in users)
        Console.WriteLine($"- {item.Id} (roles: {String.Join(';', item.Roles)})");
    // Send messages.
    Console.WriteLine("You can start writing messages. Type '/exit' to stop.");
    while (Console.ReadLine() is string message && message != "/exit")
    {
        var (left, top) = Console.GetCursorPosition();
        if (!message.StartsWith("/"))
        {
            Console.SetCursorPosition(left, top - 1);
            await hubClient.SendContentMessageAsync(room.Id, MediaTypeNames.Text.Plain, Encoding.UTF8.GetBytes(message));
        }
        else if (message.IndexOf(' ') is var index && index >= 0)
        {
            var command = message[..index];
            if (commandToContentTypeMap.TryGetValue(command, out var contentType))
            {
                var content = message[(index + 1)..].Trim('"');
                await hubClient.SendContentMessageAsync(room.Id, contentType, File.ReadAllBytes(content));
            }
            else
            {
                Console.WriteLine("invalid command");
            }
        }
        else
        {
            Console.WriteLine("invalid message");
        }
    }

    recipient.MessageReceived -= Recipient_MessageReceived;
    await hubClient.DisconnectAsync();
}

await host.WaitForShutdownAsync();

// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Grpc.Net.Client;
using LetsTalk.Models;
using LetsTalkClient = LetsTalk.Models.LetsTalk.LetsTalkClient;

var options = new GrpcChannelOptions
{
    HttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }
};

using var channel = GrpcChannel.ForAddress("http://localhost:5219", options);
var client = new LetsTalkClient(channel);

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

var person = default(Person)!;
var token = default(Token)!;
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
            var registerResponse = await client.RegisterAsync(new RegisterRequest { Username = username, Password = password });
            person = registerResponse.Person;
            token = registerResponse.Token;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
        {
            Console.WriteLine($"User '{username}' already exists.");
        }
    }
}
else
{
    // Login.
    while (token is null)
    {
        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();
        try
        {
            var loginResponse = await client.LoginAsync(new LoginRequest { Username = username, Password = password });
            person = loginResponse.Person;
            token = loginResponse.Token;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
        {
            Console.WriteLine($"Username or password incorrect.");
        }
    }
}

// Test refresh token.
var refreshResponse = await client.RefreshAsync(new RefreshRequest { Username = person.Username, RefreshToken = token.RefreshToken });
token = refreshResponse.Token;
person = refreshResponse.Person;

// Now, with token to be passed on subsequent calls, create a meta object with the header.
// This should be done at channel level, since it's basically the same for each call.
var headers = new Metadata();
headers.Add("Authorization", $"Bearer {token.AccessToken}");

//// Add own private channel. Can no longer do this, requires "Administrator" role.
//await client.PostChatAsync(new PostChatRequest { Name = $"{person.Username}'s chat" }, headers);

// List chats.
var chat = default(ChatInfo);
while (chat is null)
{
    var chats = (await client.GetChatAsync(new GetChatRequest(), headers)).Chats;
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
// Join chat and send messages.
if (chat is not null)
{
    Task.Run(async () =>
    {
        var call = client.Join(new JoinRequest { ChatId = chat.Id, Username = person.Username }, headers);
        while (await call.ResponseStream.MoveNext(CancellationToken.None))
        {
            var message = call.ResponseStream.Current;
            Console.WriteLine($"{message.Username}: {message.Text}");
        }
    })
        .ConfigureAwait(false)
        .GetAwaiter();

    // Send messages.
    while (Console.ReadLine() is string line && line != "exit")
        await client.SendAsync(new Message { ChatId = chat.Id, Username = person.Username, Text = line }, headers);
}

//// Leave chat.
//await client.LeaveAsync(new LeaveRequest { Chat = chat, Person = person }, headers);

// Logout.
await client.LogoutAsync(new LogoutRequest { Username = person.Username }, headers);

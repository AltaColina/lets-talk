// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Grpc.Net.Client;
using LetsTalk;
using LetsTalkClient = LetsTalk.LetsTalk.LetsTalkClient;

var options = new GrpcChannelOptions
{
    HttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }
};

using var channel = GrpcChannel.ForAddress("http://localhost:5219", options);
var client = new LetsTalkClient(channel);

// Register.
var user = default(User);
while (user is null)
{
    Console.Write("Username: ");
    var username = Console.ReadLine();
    Console.Write("Password: ");
    var password = Console.ReadLine();
    try
    {
        var registerResponse = await client.RegisterAsync(new RegisterRequest { Username = username, Password = password });
        user = registerResponse.User;
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
    {
        Console.WriteLine($"User '{username}' already exists.");
    }
}

// Login.
var token = default(string);
while (token is null)
{
    Console.WriteLine($"Enter password for {user.Username}");
    var password = Console.ReadLine();
    try
    {
        var loginResponse = await client.LoginAsync(new LoginRequest { User = user, Password = password });
        token = loginResponse.Token;
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
    {
        Console.WriteLine($"Username or password incorrect.");
    }
}

// Now, with token to be passed on subsequent calls, create a meta object with the header.
// This should be done at channel level, since it's basically the same for each call.
var headers = new Metadata();
headers.Add("Authorization", $"Bearer {token}");

// Add own private channel.
await client.PostChatAsync(new PostChatRequest { Name = $"{user.Username}'s chat" }, headers);

// Join a chat.
var chat = default(Chat);
while (chat is null)
{
    var chats = (await client.GetChatAsync(new GetChatRequest(), headers)).Chats;
    Console.WriteLine("Select a channel to join by typing its number.");
    for (int i = 0; i < chats.Count; ++i)
        Console.WriteLine($"{i + 1}: {chats[i].Name}");
    var input = Console.ReadLine();
    if (!Int32.TryParse(input, out int number))
        Console.WriteLine("Input must be a number.");
    else if ((number - 1) is var index && (index < 0 || index >= chats.Count))
        Console.WriteLine($"Must be a number between {1} and {chats.Count}.");
    else
        chat = chats[index];
}
Task.Run(async () =>
{
    var call = client.Join(new JoinRequest { Chat = chat, User = user }, headers);
    while (await call.ResponseStream.MoveNext(CancellationToken.None))
    {
        var message = call.ResponseStream.Current;
        Console.WriteLine($"{message.User.Username}: {message.Text}");
    }
})
    .ConfigureAwait(false)
    .GetAwaiter();

// Send messages.
while (Console.ReadLine() is string line && line != "exit")
    await client.SendAsync(new Message { Chat = chat, User = user, Text = line }, headers);

// Leave chat.
await client.LeaveAsync(new LeaveRequest { Chat = chat, User = user }, headers);

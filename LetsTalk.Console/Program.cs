﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using LetsTalk.Interfaces;
using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Console;
using LetsTalk.Dtos;
using LetsTalk.Commands.Auths;
using LetsTalk.Messaging;
using System.Text;
using System.Net.Mime;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configuration => configuration.AddContainersConfiguration("localhost", "/LetsTalk"))
    .ConfigureServices((hostContext, services) => services
        .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
        .AddLetsTalkSettings(hostContext.Configuration)
        .AddLetsTalkHttpClient(hostContext.Configuration)
        .AddLetsTalkHubClient(hostContext.Configuration)
        .AddTransient<MessageRecipient>())
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

var chats = (await hubClient.GetUserChatsAsync()).Chats;
var chat = default(ChatDto);
while (chat is null)
{
    Console.WriteLine("Select a channel to listen to by typing its number.");
    int i = 0;
    for (; i < chats.Count; ++i)
        Console.WriteLine($"{i + 1}: {chats[i].Name}");
    Console.WriteLine($"{i + 1}: Join another channel.");
    Console.WriteLine("0: Exit LetsTalk");
    var input = Console.ReadLine();
    if (!Int32.TryParse(input, out int number))
    {
        Console.WriteLine("Input must be a number.");
    }
    else if (number < 0 || number > chats.Count + 1)
    {
        Console.WriteLine($"Must be a number between {0} and {chats.Count + 1}.");
    }
    else if (number == 0)
    {
        break;
    }
    else if (number == chats.Count + 1)
    {
        var allChats = (await hubClient.GetUserAvailableChatsAsync()).Chats;
        allChats.RemoveAll(chat => settings.Authentication.User.Chats.Contains(chat.Id));
        Console.WriteLine("Available chats:");
        foreach (var item in allChats)
            Console.WriteLine($"- {item.Name} (id: {item.Id})");
        Console.WriteLine("Select chat to join. Type '/back' to go return to previous menu.");
        Console.Write("Chat Id: ");
        var chatId = Console.ReadLine()!;
        if (chatId != "/back")
        {
            var newChat = await httpClient.GetChatAsync(chatId, settings.Authentication.AccessToken.Id);
            chats.Add(newChat);
            await hubClient.JoinChatAsync(chatId);
        }
    }
    else
    {
        chat = chats[number - 1];
    }
}

static void Recipient_MessageReceived(object? sender, string message) => Console.WriteLine(message);

if (chat is not null)
{
    var recipient = host.Services.GetRequiredService<MessageRecipient>();
    recipient.ListenToChat(chat.Id);
    recipient.MessageReceived += Recipient_MessageReceived;

    var users = (await hubClient.GetLoggedChatUsersAsync(chat.Id)).Users;
    users.RemoveAll(user => user.Id == settings.Authentication.User.Id);
    Console.WriteLine($"Listening to chat {chat.Name} (id: {chat.Id}). Logged users:");
    foreach (var item in users)
        Console.WriteLine($"- {item.Id} (roles: {String.Join(';', item.Roles)})");
    // Send messages.
    Console.WriteLine("You can start writing messages. Type '/exit' to stop.");
    while (Console.ReadLine() is string message && message != "/exit")
    {
        var (left, top) = Console.GetCursorPosition();
        Console.SetCursorPosition(left, top - 1);
        await hubClient.SendChatMessageAsync(chat.Id, MediaTypeNames.Text.Plain, Encoding.UTF8.GetBytes(message));
    }

    recipient.MessageReceived -= Recipient_MessageReceived;
    await hubClient.DisconnectAsync();
}

await host.WaitForShutdownAsync();

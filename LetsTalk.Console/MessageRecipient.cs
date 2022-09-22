using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Models;

namespace LetsTalk.Console;
internal sealed class MessageRecipient : IRecipient<ChatMessage>
{
    private readonly string _chatId;

    public MessageRecipient(string chatId)
    {
        _chatId = chatId;
    }

    public void Receive(ChatMessage message)
    {
        if (message.ChatId == _chatId)
            System.Console.WriteLine($"{message.UserId}: {message.Content}");
    }
}

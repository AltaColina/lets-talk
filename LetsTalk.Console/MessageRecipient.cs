using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Models;

namespace LetsTalk.Console;
internal sealed class MessageRecipient : IRecipient<ChatMessage>
{
    public void Receive(ChatMessage message)
    {
        System.Console.WriteLine($"{message.UserId}: {message.Content}");
    }
}

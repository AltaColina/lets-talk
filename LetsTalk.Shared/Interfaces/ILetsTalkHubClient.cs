using LetsTalk.Models;
using LetsTalk.Queries.Hubs;
using System.Collections.ObjectModel;

namespace LetsTalk.Interfaces;

public interface ILetsTalkHubClient
{
    bool IsConnected { get; }

    Task ConnectAsync();

    Task DisconnectAsync();

    ObservableCollection<ChatMessage> GetChatMessages(string chatId);

    Task JoinChatAsync(string chatId);
    Task LeaveChatAsync(string chatId);

    Task SendChatMessageAsync(string chatId, string message);

    Task<GetLoggedUsersResponse> GetLoggedUsersAsync();

    Task<GetLoggedChatUsersResponse> GetLoggedChatUsersAsync(string chatId);
}

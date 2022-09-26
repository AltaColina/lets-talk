using LetsTalk.Commands.Hubs;
using LetsTalk.Queries.Chats;
using LetsTalk.Queries.Hubs;

namespace LetsTalk.Interfaces;

public interface ILetsTalkHubClient
{
    bool IsConnected { get; }
    Task ConnectAsync();
    Task DisconnectAsync();
    Task<JoinChatResponse> JoinChatAsync(string chatId);
    Task<LeaveChatResponse> LeaveChatAsync(string chatId);
    Task SendChatMessageAsync(string chatId, string message);
    Task<GetLoggedUsersResponse> GetLoggedUsersAsync();
    Task<GetLoggedChatUsersResponse> GetLoggedChatUsersAsync(string chatId);
    Task<GetUserChatsResponse> GetUserChatsAsync();
    Task<GetUserAvailableChatsResponse> GetUserAvailableChatsAsync();
}

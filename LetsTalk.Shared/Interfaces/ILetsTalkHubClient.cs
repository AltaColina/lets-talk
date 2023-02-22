using LetsTalk.Hubs.Commands;
using LetsTalk.Hubs.Queries;
using LetsTalk.Users.Queries;

namespace LetsTalk.Interfaces;

public interface ILetsTalkHubClient
{
    bool IsConnected { get; }
    Task ConnectAsync();
    Task DisconnectAsync();
    Task<JoinChatResponse> JoinChatAsync(string chatId);
    Task<LeaveChatResponse> LeaveChatAsync(string chatId);
    Task<GetLoggedUsersResponse> GetLoggedUsersAsync();
    Task<GetLoggedChatUsersResponse> GetLoggedChatUsersAsync(string chatId);
    Task<GetUserChatsResponse> GetUserChatsAsync();
    Task<GetUserAvailableChatsResponse> GetUserAvailableChatsAsync();
    Task SendChatMessageAsync(string chatId, string contentType, byte[] message);
}

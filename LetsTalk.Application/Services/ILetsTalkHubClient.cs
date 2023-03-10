using LetsTalk.Hubs.Commands;
using LetsTalk.Hubs.Queries;
using LetsTalk.Users.Queries;

namespace LetsTalk.Services;

public interface ILetsTalkHubClient
{
    bool IsConnected { get; }
    Task ConnectAsync();
    Task DisconnectAsync();
    Task<JoinRoomResponse> JoinRoomAsync(string roomId);
    Task<LeaveRoomResponse> LeaveRoomAsync(string roomId);
    Task<GetLoggedUsersResponse> GetLoggedUsersAsync();
    Task<GetLoggedRoomUsersResponse> GetLoggedRoomUsersAsync(string roomId);
    Task<GetUserRoomsResponse> GetUserRoomsAsync();
    Task<GetUserAvailableRoomsResponse> GetUserAvailableRoomsAsync();
    Task SendContentMessageAsync(string roomId, string contentType, byte[] message);
}

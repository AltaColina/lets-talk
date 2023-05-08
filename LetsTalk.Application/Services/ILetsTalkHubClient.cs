using LetsTalk.Hubs.Commands;
using LetsTalk.Hubs.Queries;
using LetsTalk.Rooms.Queries;

namespace LetsTalk.Services;

public interface ILetsTalkHubClient
{
    bool IsConnected { get; }
    Task ConnectAsync(Func<Task<string?>>? accessTokenProvider = null);
    Task DisconnectAsync();
    Task<JoinRoomResponse> JoinRoomAsync(string roomId);
    Task<LeaveRoomResponse> LeaveRoomAsync(string roomId);
    Task<GetUsersLoggedInResponse> GetUsersLoggedInAsync();
    Task<GetUsersLoggedInRoomResponse> GetUsersLoggedInRoomAsync(string roomId);
    Task<GetRoomsWithUserResponse> GetRoomsWithUserAsync();
    Task<GetRoomsWithoutUserResponse> GetRoomsWithoutUserAsync();
    Task SendContentMessageAsync(string roomId, string contentType, byte[] message);
}

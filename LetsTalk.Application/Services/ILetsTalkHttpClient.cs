using LetsTalk.Roles;
using LetsTalk.Roles.Commands;
using LetsTalk.Roles.Queries;
using LetsTalk.Rooms;
using LetsTalk.Rooms.Commands;
using LetsTalk.Rooms.Queries;
using LetsTalk.Security;
using LetsTalk.Users;
using LetsTalk.Users.Commands;
using LetsTalk.Users.Queries;

namespace LetsTalk.Services;

public interface ILetsTalkHttpClient
{
    Task<Authentication> RegisterAsync(string username, string password);
    Task<Authentication> LoginAsync(string username, string password);
    Task<Authentication> RefreshAsync(string username, string refreshToken);

    Task<GetRoomsResponse> GetRoomsAsync(string accessToken);
    Task<RoomDto> GetRoomAsync(string roomId, string accessToken);
    Task<RoomDto> CreateRoomAsync(CreateRoomCommand room, string accessToken);
    Task<RoomDto> UpdateRoomAsync(UpdateRoomCommand room, string accessToken);
    Task DeleteRoomAsync(string roomId, string accessToken);
    Task<GetRoomUsersResponse> GetRoomUsersAsync(string roomId, string accessToken);

    Task<GetRolesResponse> GetRolesAsync(string accessToken);
    Task<RoleDto> GetRoleAsync(string roleId, string accessToken);
    Task<RoleDto> CreateRoleAsync(CreateRoleCommand role, string accessToken);
    Task<RoleDto> UpdateRoleAsync(UpdateRoleCommand role, string accessToken);
    Task DeleteRoleAsync(string roleId, string accessToken);
    Task<GetRoleUsersResponse> GetRoleUsersAsync(string roleId, string accessToken);

    Task<GetUsersResponse> GetUsersAsync(string accessToken);
    Task<UserDto> GetUserAsync(string userId, string accessToken);
    Task<UserDto> UpdateUserAsync(UpdateUserCommand user, string accessToken);
    Task DeleteUserAsync(string userId, string accessToken);
    Task<GetUserRoomsResponse> GetUserRoomsAsync(string userId, string accessToken);
}
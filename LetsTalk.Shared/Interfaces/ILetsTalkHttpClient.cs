using LetsTalk.Rooms;
using LetsTalk.Rooms.Commands;
using LetsTalk.Rooms.Queries;
using LetsTalk.Roles;
using LetsTalk.Roles.Commands;
using LetsTalk.Roles.Queries;
using LetsTalk.Security;
using LetsTalk.Security.Commands;
using LetsTalk.Users;
using LetsTalk.Users.Commands;
using LetsTalk.Users.Queries;

namespace LetsTalk.Interfaces;

public interface ILetsTalkHttpClient
{
    Task<Authentication> RegisterAsync(RegisterCommand request);
    Task<Authentication> LoginAsync(LoginCommand request);
    Task<Authentication> RefreshAsync(RefreshCommand request);

    Task<GetRoomsResponse> GetRoomsAsync(string token);
    Task<RoomDto> GetRoomAsync(string roomId, string token);
    Task<RoomDto> CreateRoomAsync(CreateRoomCommand room, string token);
    Task<RoomDto> UpdateRoomAsync(UpdateRoomCommand room, string token);
    Task DeleteRoomAsync(string roomId, string token);
    Task<GetRoomUsersResponse> GetRoomUsersAsync(string roomId, string token);

    Task<GetRolesResponse> GetRolesAsync(string token);
    Task<RoleDto> GetRoleAsync(string roleId, string token);
    Task<RoleDto> CreateRoleAsync(CreateRoleCommand role, string token);
    Task<RoleDto> UpdateRoleAsync(UpdateRoleCommand role, string token);
    Task DeleteRoleAsync(string roleId, string token);
    Task<GetRoleUsersResponse> GetRoleUsersAsync(string roleId, string token);

    Task<GetUsersResponse> GetUsersAsync(string token);
    Task<UserDto> GetUserAsync(string userId, string token);
    Task<UserDto> UpdateUserAsync(UpdateUserCommand user, string token);
    Task DeleteUserAsync(string userId, string token);
    Task<GetUserRoomsResponse> GetUserRoomsAsync(string userId, string token);
}
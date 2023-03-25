using LetsTalk.Roles;
using LetsTalk.Roles.Commands;
using LetsTalk.Roles.Queries;
using LetsTalk.Rooms;
using LetsTalk.Rooms.Commands;
using LetsTalk.Rooms.Queries;
using LetsTalk.Users;
using LetsTalk.Users.Commands;
using LetsTalk.Users.Queries;

namespace LetsTalk.Services;

public interface ILetsTalkHttpClient
{
    Task<string> PingAsync();

    Task<GetRoomsResponse> GetRoomsAsync();
    Task<RoomDto> GetRoomAsync(string roomId);
    Task<RoomDto> CreateRoomAsync(CreateRoomCommand room);
    Task<RoomDto> UpdateRoomAsync(UpdateRoomCommand room);
    Task DeleteRoomAsync(string roomId);
    Task<GetRoomUsersResponse> GetRoomUsersAsync(string roomId);

    Task<GetRolesResponse> GetRolesAsync();
    Task<RoleDto> GetRoleAsync(string roleId);
    Task<RoleDto> CreateRoleAsync(CreateRoleCommand role);
    Task<RoleDto> UpdateRoleAsync(UpdateRoleCommand role);
    Task DeleteRoleAsync(string roleId);
    Task<GetRoleUsersResponse> GetRoleUsersAsync(string roleId);

    Task<GetUsersResponse> GetUsersAsync();
    Task<UserDto> GetUserAsync(string userId);
    Task<UserDto> CreateUserAsync(CreateUserCommand user);
    Task<UserDto> UpdateUserAsync(UpdateUserCommand user);
    Task DeleteUserAsync(string userId);
    Task<GetRoomsWithUserResponse> GetUserRoomsAsync(string userId);
}
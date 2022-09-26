using LetsTalk.Commands.Auths;
using LetsTalk.Commands.Chats;
using LetsTalk.Commands.Roles;
using LetsTalk.Commands.Users;
using LetsTalk.Dtos;
using LetsTalk.Models;
using LetsTalk.Queries.Chats;
using LetsTalk.Queries.Roles;
using LetsTalk.Queries.Users;

namespace LetsTalk.Interfaces;

public interface ILetsTalkHttpClient
{
    Task<Authentication> RegisterAsync(RegisterRequest request);
    Task<Authentication> LoginAsync(LoginRequest request);
    Task<Authentication> RefreshAsync(RefreshRequest request);

    Task<GetChatsResponse> GetChatsAsync(string token);
    Task<ChatDto> GetChatAsync(string chatId, string token);
    Task<ChatDto> CreateChatAsync(CreateChatRequest chat, string token);
    Task<ChatDto> UpdateChatAsync(UpdateChatRequest chat, string token);
    Task DeleteChatAsync(string chatId, string token);
    Task<GetChatUsersResponse> GetChatUsersAsync(string chatId, string token);

    Task<GetRolesResponse> GetRolesAsync(string token);
    Task<RoleDto> GetRoleAsync(string roleId, string token);
    Task<RoleDto> CreateRoleAsync(CreateRoleRequest role, string token);
    Task<RoleDto> UpdateRoleAsync(UpdateRoleRequest role, string token);
    Task DeleteRoleAsync(string roleId, string token);
    Task<GetRoleUsersResponse> GetRoleUsersAsync(string roleId, string token);

    Task<GetUsersResponse> GetUsersAsync(string token);
    Task<UserDto> GetUserAsync(string userId, string token);
    Task<UserDto> UpdateUserAsync(UpdateUserRequest user, string token);
    Task DeleteUserAsync(string userId, string token);
    Task<GetUserChatsResponse> GetUserChatsAsync(string userId, string token);
}
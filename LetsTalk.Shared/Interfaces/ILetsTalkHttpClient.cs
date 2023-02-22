using LetsTalk.Chats;
using LetsTalk.Chats.Commands;
using LetsTalk.Chats.Queries;
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

    Task<GetChatsResponse> GetChatsAsync(string token);
    Task<ChatDto> GetChatAsync(string chatId, string token);
    Task<ChatDto> CreateChatAsync(CreateChatCommand chat, string token);
    Task<ChatDto> UpdateChatAsync(UpdateChatCommand chat, string token);
    Task DeleteChatAsync(string chatId, string token);
    Task<GetChatUsersResponse> GetChatUsersAsync(string chatId, string token);

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
    Task<GetUserChatsResponse> GetUserChatsAsync(string userId, string token);
}
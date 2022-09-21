using LetsTalk.Dtos.Auths;
using LetsTalk.Dtos.Chats;
using LetsTalk.Dtos.Roles;
using LetsTalk.Dtos.Users;
using LetsTalk.Models;

namespace LetsTalk.Interfaces;

public interface ILetsTalkHttpClient
{
    Task<Authentication> LoginAsync(LoginRequest request);
    Task<Authentication> RefreshAsync(RefreshRequest request);
    Task<Authentication> RegisterAsync(RegisterRequest request);

    Task DeleteChatAsync(string chatId, string token);
    Task<GetChatsResponse> GetChatsAsync(string token);
    Task<ChatDto> GetChatAsync(string chatId, string token);
    Task CreateChatAsync(Chat chat, string token);
    Task UpdateChatAsync(Chat chat, string token);
    Task<GetChatUsersResponse> GetChatUsersAsync(string chatId, string token);

    Task DeleteRoleAsync(string roleId, string token);
    Task<GetRolesResponse> GetRolesAsync(string token);
    Task<RoleDto> GetRoleAsync(string roleId, string token);
    Task CreateRoleAsync(Role role, string token);
    Task UpdateRoleAsync(Role role, string token);
    Task<GetRoleUsersResponse> GetRoleUsersAsync(string roleId, string token);

    Task DeleteUserAsync(string userId, string token);
    Task<GetUsersResponse> GetUsersAsync(string token);
    Task<UserDto> GetUserAsync(string userId, string token);
    Task UpdateUserAsync(User user, string token);
}
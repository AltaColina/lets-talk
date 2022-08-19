using LetsTalk.Models;
using LetsTalk.Models.Auths;
using LetsTalk.Models.Chats;
using LetsTalk.Models.Roles;
using LetsTalk.Models.Users;

namespace LetsTalk.Interfaces;

public interface ILetsTalkHttpClient
{
    Task ChatDeleteAsync(string chatId, string token);
    Task<ChatGetResponse> ChatGetAsync(string token);
    Task<ChatGetResponse> ChatGetAsync(string chatId, string token);
    Task ChatPostAsync(Chat chat, string token);
    Task ChatPutAsync(Chat chat, string token);
    Task<ChatUserGetResponse> ChatUserGetAsync(string chatId, string token);
    Task ChatUserPutAsync(string chatId, string userId, string token);
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);
    Task<AuthenticationResponse> RefreshAsync(RefreshRequest request);
    Task<AuthenticationResponse> RegisterAsync(RegisterRequest request);
    Task RoleDeleteAsync(string roleId, string token);
    Task<RoleGetResponse> RoleGetAsync(string token);
    Task<RoleGetResponse> RoleGetAsync(string roleId, string token);
    Task RolePostAsync(Role role, string token);
    Task RolePutAsync(Role role, string token);
    Task<RoleUserGetResponse> RoleUserGetAsync(string roleId, string token);
    Task RoleUserPutAsync(string roleId, string userId, string token);
    Task<UserChatGetResponse> UserChatGetAsync(string userId, string token);
    Task UserChatPutAsync(string userId, string chatId, string token);
    Task UserDeleteAsync(string userId, string token);
    Task<UserGetResponse> UserGetAsync(string token);
    Task<UserGetResponse> UserGetAsync(string userId, string token);
    Task UserPostAsync(User user, string token);
    Task UserPutAsync(User user, string token);
    Task<UserRoleGetResponse> UserRoleGetAsync(string userId, string token);
    Task UserRolePutAsync(string userId, string roleId, string token);
}
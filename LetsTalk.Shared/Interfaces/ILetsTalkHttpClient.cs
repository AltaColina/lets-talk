using LetsTalk.Models;
using LetsTalk.Models.Auths;
using LetsTalk.Models.Chats;
using LetsTalk.Models.Roles;
using LetsTalk.Models.Users;

namespace LetsTalk.Interfaces;

public interface ILetsTalkHttpClient
{
    Task ChatDeleteAsync(string chatId);
    Task<ChatGetResponse> ChatGetAsync();
    Task<ChatGetResponse> ChatGetAsync(string chatId);
    Task ChatPostAsync(Chat chat);
    Task ChatPutAsync(Chat chat);
    Task<ChatUserGetResponse> ChatUserGetAsync(string chatId);
    Task ChatUserPutAsync(string chatId, string userId);
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);
    Task<AuthenticationResponse> RefreshAsync(RefreshRequest request);
    Task<AuthenticationResponse> RegisterAsync(RegisterRequest request);
    Task RoleDeleteAsync(string roleId);
    Task<RoleGetResponse> RoleGetAsync();
    Task<RoleGetResponse> RoleGetAsync(string roleId);
    Task RolePostAsync(Role role);
    Task RolePutAsync(Role role);
    Task<RoleUserGetResponse> RoleUserGetAsync(string roleId);
    Task RoleUserPutAsync(string roleId, string userId);
    Task<UserChatGetResponse> UserChatGetAsync(string userId);
    Task UserChatPutAsync(string userId, string chatId);
    Task UserDeleteAsync(string userId);
    Task<UserGetResponse> UserGetAsync();
    Task<UserGetResponse> UserGetAsync(string userId);
    Task UserPostAsync(User user);
    Task UserPutAsync(User user);
    Task<UserRoleGetResponse> UserRoleGetAsync(string userId);
    Task UserRolePutAsync(string userId, string roleId);
}
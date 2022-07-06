using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Models.Auths;
using LetsTalk.Models.Chats;
using LetsTalk.Models.Roles;
using LetsTalk.Models.Users;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LetsTalk.Services;

internal sealed class LetsTalkHttpClient : ILetsTalkHttpClient
{
    private readonly HttpClient _httpClient;
    private AuthenticationResponse? _authentication;

    private AuthenticationResponse? Authentication
    {
        get => _authentication;
        set
        {
            _authentication = value;
            _httpClient.DefaultRequestHeaders.Authorization = _authentication is not null
                ? new AuthenticationHeaderValue("Bearer", _authentication.AccessToken.Id)
                : null;
        }
    }

    public LetsTalkHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private async Task RefreshAuthenticationAsync()
    {
        if (Authentication is not null)
        {
            var dateTime = DateTimeOffset.UtcNow;
            if (Authentication.AccessToken.ExpiresIn < dateTime && Authentication.RefreshToken.ExpiresIn > dateTime)
            {
                Authentication = await RefreshAsync(new RefreshRequest
                {
                    RefreshToken = Authentication.RefreshToken.Id,
                    Username = Authentication.Person.Username
                });
            }
        }
    }

    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        response.EnsureSuccessStatusCode();
        Authentication = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        return Authentication!;
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        response.EnsureSuccessStatusCode();
        Authentication = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        return Authentication!;
    }

    public async Task<AuthenticationResponse> RefreshAsync(RefreshRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);
        response.EnsureSuccessStatusCode();
        Authentication = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        return Authentication!;
    }

    public async Task<ChatGetResponse> ChatGetAsync()
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<ChatGetResponse>("api/chat"))!;
    }

    public async Task<ChatGetResponse> ChatGetAsync(string chatId)
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<ChatGetResponse>($"api/chat/{chatId}"))!;
    }

    public async Task ChatPostAsync(Chat chat)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PostAsJsonAsync("api/chat", chat);
        response.EnsureSuccessStatusCode();
    }

    public async Task ChatPutAsync(Chat chat)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PutAsJsonAsync("api/chat", chat);
        response.EnsureSuccessStatusCode();
    }

    public async Task ChatDeleteAsync(string chatId)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.DeleteAsync($"api/chat/{chatId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<ChatUserGetResponse> ChatUserGetAsync(string chatId)
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<ChatUserGetResponse>($"api/chat/{chatId}/user"))!;
    }

    public async Task ChatUserPutAsync(string chatId, string userId)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PutAsJsonAsync($"api/chat/{chatId}/user", userId);
        response.EnsureSuccessStatusCode();
    }

    public async Task<RoleGetResponse> RoleGetAsync()
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<RoleGetResponse>("api/role"))!;
    }

    public async Task<RoleGetResponse> RoleGetAsync(string roleId)
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<RoleGetResponse>($"api/role/{roleId}"))!;
    }

    public async Task RolePostAsync(Role role)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PostAsJsonAsync("api/role", role);
        response.EnsureSuccessStatusCode();
    }

    public async Task RolePutAsync(Role role)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PutAsJsonAsync("api/role", role);
        response.EnsureSuccessStatusCode();
    }

    public async Task RoleDeleteAsync(string roleId)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.DeleteAsync($"api/role/{roleId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<RoleUserGetResponse> RoleUserGetAsync(string roleId)
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<RoleUserGetResponse>($"api/role/{roleId}/user"))!;
    }

    public async Task RoleUserPutAsync(string roleId, string userId)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PutAsJsonAsync($"api/role/{roleId}/user", userId);
        response.EnsureSuccessStatusCode();
    }

    public async Task<UserGetResponse> UserGetAsync()
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<UserGetResponse>("api/user"))!;
    }

    public async Task<UserGetResponse> UserGetAsync(string userId)
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<UserGetResponse>($"api/user/{userId}"))!;
    }

    public async Task UserPostAsync(User user)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PostAsJsonAsync("api/user", user);
        response.EnsureSuccessStatusCode();
    }

    public async Task UserPutAsync(User user)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PutAsJsonAsync("api/user", user);
        response.EnsureSuccessStatusCode();
    }

    public async Task UserDeleteAsync(string userId)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.DeleteAsync($"api/user/{userId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<UserChatGetResponse> UserChatGetAsync(string userId)
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<UserChatGetResponse>($"api/user/{userId}/chat"))!;
    }

    public async Task UserChatPutAsync(string userId, string chatId)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PutAsJsonAsync($"api/user/{userId}/chat", chatId);
        response.EnsureSuccessStatusCode();
    }

    public async Task<UserRoleGetResponse> UserRoleGetAsync(string userId)
    {
        await RefreshAuthenticationAsync();
        return (await _httpClient.GetFromJsonAsync<UserRoleGetResponse>($"api/user/{userId}/role"))!;
    }

    public async Task UserRolePutAsync(string userId, string roleId)
    {
        await RefreshAuthenticationAsync();
        var response = await _httpClient.PutAsJsonAsync($"api/user/{userId}/role", roleId);
        response.EnsureSuccessStatusCode();
    }
}
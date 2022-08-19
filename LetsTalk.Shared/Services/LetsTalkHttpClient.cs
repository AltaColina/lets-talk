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

    public LetsTalkHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    public async Task<AuthenticationResponse> RefreshAsync(RefreshRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    public async Task<ChatGetResponse> ChatGetAsync(string token) =>
        await GetAsync<ChatGetResponse>("api/chat", token);

    public async Task<ChatGetResponse> ChatGetAsync(string chatId, string token) =>
        await GetAsync<ChatGetResponse>($"api/chat/{chatId}", token);

    public async Task ChatPostAsync(Chat chat, string token) =>
        await PostAsync("api/chat", chat, token);

    public async Task ChatPutAsync(Chat chat, string token) =>
        await PutAsync("api/chat", chat, token);

    public async Task ChatDeleteAsync(string chatId, string token) =>
        await DeleteAsync($"api/chat/{chatId}", token);

    public async Task<ChatUserGetResponse> ChatUserGetAsync(string chatId, string token) =>
        await GetAsync<ChatUserGetResponse>($"api/chat/{chatId}/user", token);

    public async Task ChatUserPutAsync(string chatId, string userId, string token) =>
        await PutAsync($"api/chat/{chatId}/user", userId, token);

    public async Task<RoleGetResponse> RoleGetAsync(string token) =>
        await GetAsync<RoleGetResponse>("api/role", token);

    public async Task<RoleGetResponse> RoleGetAsync(string roleId, string token) =>
        await GetAsync<RoleGetResponse>($"api/role/{roleId}", token);

    public async Task RolePostAsync(Role role, string token) =>
        await PostAsync("api/role", role, token);

    public async Task RolePutAsync(Role role, string token) =>
        await PutAsync("api/role", role, token);

    public async Task RoleDeleteAsync(string roleId, string token) =>
        await DeleteAsync($"api/role/{roleId}", token);

    public async Task<RoleUserGetResponse> RoleUserGetAsync(string roleId, string token) =>
        await GetAsync<RoleUserGetResponse>($"api/role/{roleId}/user", token);

    public async Task RoleUserPutAsync(string roleId, string userId, string token) =>
        await PutAsync($"api/role/{roleId}/user", userId, token);

    public async Task<UserGetResponse> UserGetAsync(string token) =>
        await GetAsync<UserGetResponse>("api/user", token);

    public async Task<UserGetResponse> UserGetAsync(string userId, string token) =>
        await GetAsync<UserGetResponse>($"api/user/{userId}", token);

    public async Task UserPostAsync(User user, string token) =>
        await PostAsync("api/user", user, token);

    public async Task UserPutAsync(User user, string token) =>
        await PutAsync("api/user", user, token);

    public async Task UserDeleteAsync(string userId, string token) =>
        await DeleteAsync($"api/user/{userId}", token);

    public async Task<UserChatGetResponse> UserChatGetAsync(string userId, string token) =>
        await GetAsync<UserChatGetResponse>($"api/user/{userId}/chat", token);

    public async Task UserChatPutAsync(string userId, string chatId, string token) =>
        await PutAsync($"api/user/{userId}/chat", chatId, token);

    public async Task<UserRoleGetResponse> UserRoleGetAsync(string userId, string token) =>
        await GetAsync<UserRoleGetResponse>($"api/user/{userId}/role", token);

    public async Task UserRolePutAsync(string userId, string roleId, string token) =>
        await PutAsync($"api/user/{userId}/role", token, roleId);

    private async Task<T> GetAsync<T>(string uri, string token)
    {
        var response = await _httpClient.SendAsync(CreateRequest<T>(HttpMethod.Get, uri, token));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private async Task PostAsync<T>(string uri, T? value, string token)
    {
        var response = await _httpClient.SendAsync(CreateRequest<T>(HttpMethod.Post, uri, token, value));
        response.EnsureSuccessStatusCode();
    }

    private async Task PutAsync<T>(string uri, T? value, string token)
    {
        var response = await _httpClient.SendAsync(CreateRequest<T>(HttpMethod.Put, uri, token, value));
        response.EnsureSuccessStatusCode();
    }

    private async Task DeleteAsync(string uri, string token)
    {
        var response = await _httpClient.SendAsync(CreateRequest<int>(HttpMethod.Delete, uri, token));
        response.EnsureSuccessStatusCode();
    }

    private static HttpRequestMessage CreateRequest<T>(HttpMethod method, string uri, string token, T? value = default)
    {
        var request = new HttpRequestMessage(method, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(value);
        return request;
    }
}
using LetsTalk.Commands.Auths;
using LetsTalk.Commands.Chats;
using LetsTalk.Commands.Roles;
using LetsTalk.Commands.Users;
using LetsTalk.Dtos;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Queries.Chats;
using LetsTalk.Queries.Roles;
using LetsTalk.Queries.Users;
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

    public async Task<Authentication> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Authentication>())!;
    }

    public async Task<Authentication> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Authentication>())!;
    }

    public async Task<Authentication> RefreshAsync(RefreshRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Authentication>())!;
    }

    public async Task<GetChatsResponse> GetChatsAsync(string token) =>
        await GetAsync<GetChatsResponse>("api/chat", token);

    public async Task<ChatDto> GetChatAsync(string chatId, string token) =>
        await GetAsync<ChatDto>($"api/chat/{chatId}", token);

    public async Task<ChatDto> CreateChatAsync(CreateChatRequest chat, string token) =>
        await PostAsync<CreateChatRequest, ChatDto>("api/chat", chat, token);

    public async Task<ChatDto> UpdateChatAsync(UpdateChatRequest chat, string token) =>
        await PutAsync<UpdateChatRequest, ChatDto>("api/chat", chat, token);

    public async Task DeleteChatAsync(string chatId, string token) =>
        await DeleteAsync($"api/chat/{chatId}", token);

    public async Task<GetChatUsersResponse> GetChatUsersAsync(string chatId, string token) =>
        await GetAsync<GetChatUsersResponse>($"api/chat/{chatId}/user", token);

    public async Task<GetRolesResponse> GetRolesAsync(string token) =>
        await GetAsync<GetRolesResponse>("api/role", token);

    public async Task<RoleDto> GetRoleAsync(string roleId, string token) =>
        await GetAsync<RoleDto>($"api/role/{roleId}", token);

    public async Task<RoleDto> CreateRoleAsync(CreateRoleRequest role, string token) =>
        await PostAsync<CreateRoleRequest, RoleDto>("api/role", role, token);

    public async Task<RoleDto> UpdateRoleAsync(UpdateRoleRequest role, string token) =>
        await PutAsync<UpdateRoleRequest, RoleDto>("api/role", role, token);

    public async Task DeleteRoleAsync(string roleId, string token) =>
        await DeleteAsync($"api/role/{roleId}", token);

    public async Task<GetRoleUsersResponse> GetRoleUsersAsync(string roleId, string token) =>
        await GetAsync<GetRoleUsersResponse>($"api/role/{roleId}/user", token);

    public async Task<GetUsersResponse> GetUsersAsync(string token) =>
        await GetAsync<GetUsersResponse>("api/user", token);

    public async Task<UserDto> GetUserAsync(string userId, string token) =>
        await GetAsync<UserDto>($"api/user/{userId}", token);

    public async Task<UserDto> UpdateUserAsync(UpdateUserRequest user, string token) =>
        await PutAsync<UpdateUserRequest, UserDto>("api/user", user, token);

    public async Task DeleteUserAsync(string userId, string token) =>
        await DeleteAsync($"api/user/{userId}", token);

    public async Task<GetUserChatsResponse> GetUserChatsAsync(string userId, string token) =>
        await GetAsync<GetUserChatsResponse>($"api/user/{userId}/chat", token);

    private async Task<T> GetAsync<T>(string uri, string token)
    {
        var response = await _httpClient.SendAsync(CreateRequest<T>(HttpMethod.Get, uri, token));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private async Task<T2> PostAsync<T1, T2>(string uri, T1? value, string token)
    {
        var response = await _httpClient.SendAsync(CreateRequest<T1>(HttpMethod.Post, uri, token, value));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T2>())!;
    }

    private async Task<T2> PutAsync<T1, T2>(string uri, T1? value, string token)
    {
        var response = await _httpClient.SendAsync(CreateRequest<T1>(HttpMethod.Put, uri, token, value));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T2>())!;
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
using LetsTalk.Roles;
using LetsTalk.Roles.Commands;
using LetsTalk.Roles.Queries;
using LetsTalk.Rooms;
using LetsTalk.Rooms.Commands;
using LetsTalk.Rooms.Queries;
using LetsTalk.Security;
using LetsTalk.Security.Commands;
using LetsTalk.Users;
using LetsTalk.Users.Commands;
using LetsTalk.Users.Queries;
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

    public async Task<Authentication> RegisterAsync(string username, string password)
    {
        var request = new RegisterCommand { Username = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Authentication>())!;
    }

    public async Task<Authentication> LoginAsync(string username, string password)
    {
        var request = new LoginCommand { Username = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Authentication>())!;
    }

    public async Task<Authentication> RefreshAsync(string username, string refreshToken)
    {
        var request = new RefreshCommand { Username = username, RefreshToken = refreshToken };
        var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Authentication>())!;
    }

    public async Task<GetRoomsResponse> GetRoomsAsync(string accessToken) =>
        await GetAsync<GetRoomsResponse>("api/room", accessToken);

    public async Task<RoomDto> GetRoomAsync(string roomId, string accessToken) =>
        await GetAsync<RoomDto>($"api/room/{roomId}", accessToken);

    public async Task<RoomDto> CreateRoomAsync(CreateRoomCommand room, string accessToken) =>
        await PostAsync<CreateRoomCommand, RoomDto>("api/room", room, accessToken);

    public async Task<RoomDto> UpdateRoomAsync(UpdateRoomCommand room, string accessToken) =>
        await PutAsync<UpdateRoomCommand, RoomDto>("api/room", room, accessToken);

    public async Task DeleteRoomAsync(string roomId, string accessToken) =>
        await DeleteAsync($"api/room/{roomId}", accessToken);

    public async Task<GetRoomUsersResponse> GetRoomUsersAsync(string roomId, string accessToken) =>
        await GetAsync<GetRoomUsersResponse>($"api/room/{roomId}/user", accessToken);

    public async Task<GetRolesResponse> GetRolesAsync(string accessToken) =>
        await GetAsync<GetRolesResponse>("api/role", accessToken);

    public async Task<RoleDto> GetRoleAsync(string roleId, string accessToken) =>
        await GetAsync<RoleDto>($"api/role/{roleId}", accessToken);

    public async Task<RoleDto> CreateRoleAsync(CreateRoleCommand role, string accessToken) =>
        await PostAsync<CreateRoleCommand, RoleDto>("api/role", role, accessToken);

    public async Task<RoleDto> UpdateRoleAsync(UpdateRoleCommand role, string accessToken) =>
        await PutAsync<UpdateRoleCommand, RoleDto>("api/role", role, accessToken);

    public async Task DeleteRoleAsync(string roleId, string accessToken) =>
        await DeleteAsync($"api/role/{roleId}", accessToken);

    public async Task<GetRoleUsersResponse> GetRoleUsersAsync(string roleId, string accessToken) =>
        await GetAsync<GetRoleUsersResponse>($"api/role/{roleId}/user", accessToken);

    public async Task<GetUsersResponse> GetUsersAsync(string accessToken) =>
        await GetAsync<GetUsersResponse>("api/user", accessToken);

    public async Task<UserDto> GetUserAsync(string userId, string accessToken) =>
        await GetAsync<UserDto>($"api/user/{userId}", accessToken);

    public async Task<UserDto> UpdateUserAsync(UpdateUserCommand user, string accessToken) =>
        await PutAsync<UpdateUserCommand, UserDto>("api/user", user, accessToken);

    public async Task DeleteUserAsync(string userId, string accessToken) =>
        await DeleteAsync($"api/user/{userId}", accessToken);

    public async Task<GetUserRoomsResponse> GetUserRoomsAsync(string userId, string accessToken) =>
        await GetAsync<GetUserRoomsResponse>($"api/user/{userId}/room", accessToken);

    private async Task<T> GetAsync<T>(string uri, string accessToken)
    {
        var response = await _httpClient.SendAsync(CreateRequest<T>(HttpMethod.Get, uri, accessToken));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private async Task<T2> PostAsync<T1, T2>(string uri, T1? value, string accessToken)
    {
        var response = await _httpClient.SendAsync(CreateRequest<T1>(HttpMethod.Post, uri, accessToken, value));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T2>())!;
    }

    private async Task<T2> PutAsync<T1, T2>(string uri, T1? value, string accessToken)
    {
        var response = await _httpClient.SendAsync(CreateRequest<T1>(HttpMethod.Put, uri, accessToken, value));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T2>())!;
    }

    private async Task DeleteAsync(string uri, string accessToken)
    {
        var response = await _httpClient.SendAsync(CreateRequest<int>(HttpMethod.Delete, uri, accessToken));
        response.EnsureSuccessStatusCode();
    }

    private static HttpRequestMessage CreateRequest<T>(HttpMethod method, string uri, string accessToken, T? value = default)
    {
        var request = new HttpRequestMessage(method, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = JsonContent.Create(value);
        return request;
    }
}
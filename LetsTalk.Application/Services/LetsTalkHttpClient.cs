using LetsTalk.Roles;
using LetsTalk.Roles.Commands;
using LetsTalk.Roles.Queries;
using LetsTalk.Rooms;
using LetsTalk.Rooms.Commands;
using LetsTalk.Rooms.Queries;
using LetsTalk.Users;
using LetsTalk.Users.Commands;
using LetsTalk.Users.Queries;
using System.Net.Http.Json;

namespace LetsTalk.Services;

internal sealed class LetsTalkHttpClient : ILetsTalkHttpClient
{
    public HttpClient HttpClient { get; }

    public LetsTalkHttpClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public async Task<string> GreetAsync() =>
        await GetStringAsync("api/greet");

    public async Task<GetRoomsResponse> GetRoomsAsync() =>
        await GetJsonAsync<GetRoomsResponse>("api/room");

    public async Task<RoomDto> GetRoomAsync(string roomId) =>
        await GetJsonAsync<RoomDto>($"api/room/{roomId}");

    public async Task<RoomDto> CreateRoomAsync(CreateRoomCommand room) =>
        await PostJsonAsync<CreateRoomCommand, RoomDto>("api/room", room);

    public async Task<RoomDto> UpdateRoomAsync(UpdateRoomCommand room) =>
        await PutJsonAsync<UpdateRoomCommand, RoomDto>("api/room", room);

    public async Task DeleteRoomAsync(string roomId) =>
        await DeleteAsync($"api/room/{roomId}");

    public async Task<GetRoomUsersResponse> GetRoomUsersAsync(string roomId) =>
        await GetJsonAsync<GetRoomUsersResponse>($"api/room/{roomId}/user");

    public async Task<GetRolesResponse> GetRolesAsync() =>
        await GetJsonAsync<GetRolesResponse>("api/role");

    public async Task<RoleDto> GetRoleAsync(string roleId) =>
        await GetJsonAsync<RoleDto>($"api/role/{roleId}");

    public async Task<RoleDto> CreateRoleAsync(CreateRoleCommand role) =>
        await PostJsonAsync<CreateRoleCommand, RoleDto>("api/role", role);

    public async Task<RoleDto> UpdateRoleAsync(UpdateRoleCommand role) =>
        await PutJsonAsync<UpdateRoleCommand, RoleDto>("api/role", role);

    public async Task DeleteRoleAsync(string roleId) =>
        await DeleteAsync($"api/role/{roleId}");

    public async Task<GetUsersResponse> GetUsersAsync() =>
        await GetJsonAsync<GetUsersResponse>("api/user");

    public async Task<UserDto> GetUserAsync(string userId) =>
        await GetJsonAsync<UserDto>($"api/user/{userId}");

    public async Task<UserDto> CreateUserAsync(CreateUserCommand user) =>
        await PostJsonAsync<CreateUserCommand, UserDto>($"api/user", user);

    public async Task<UserDto> UpdateUserAsync(UpdateUserCommand user) =>
        await PutJsonAsync<UpdateUserCommand, UserDto>("api/user", user);

    public async Task DeleteUserAsync(string userId) =>
        await DeleteAsync($"api/user/{userId}");

    private async Task<string> GetStringAsync(string uri) => await HttpClient.GetStringAsync(uri);

    private async Task<T> GetJsonAsync<T>(string uri) => (await HttpClient.GetFromJsonAsync<T>(uri))!;

    private async Task<T2> PostJsonAsync<T1, T2>(string uri, T1? value)
    {
        var response = await HttpClient.PostAsJsonAsync(uri, value);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T2>())!;
    }

    private async Task<T2> PutJsonAsync<T1, T2>(string uri, T1? value)
    {
        var response = await HttpClient.PutAsJsonAsync(uri, value);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T2>())!;
    }

    private async Task DeleteAsync(string uri)
    {
        var response = await HttpClient.DeleteAsync(uri);
        response.EnsureSuccessStatusCode();
    }
}
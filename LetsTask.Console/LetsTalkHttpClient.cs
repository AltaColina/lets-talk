using LetsTalk.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LetsTask.Console;

public sealed class LetsTalkHttpClient
{
    private readonly HttpClient _httpClient;
    private Token? _accessToken;

    public Token? AcessToken
    {
        get => _accessToken;
        set
        {
            _accessToken = value;
            if (_accessToken is null)
                _httpClient.DefaultRequestHeaders.Authorization = null;
            else
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken.Id);
        }
    }

    public LetsTalkHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest request)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/register", httpContent);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/login", httpContent);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    public async Task<AuthenticationResponse> RefreshAsync(RefreshRequest request)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/refresh", httpContent);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    public async Task<ChatGetResponse> ChatGetAsync(ChatGetRequest request)
    {
        var url = !String.IsNullOrWhiteSpace(request.ChatId)
            ? $"api/chat/{request.ChatId}"
            : "api/chat";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ChatGetResponse>())!;
    }

    public async Task ChatPostAsync(ChatPostRequest request)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/chat", httpContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task ChatPutAsync(ChatPutRequest request)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/chat", httpContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task ChatDeleteAsync(ChatDeleteRequest request)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/chat", httpContent);
        response.EnsureSuccessStatusCode();
    }
}
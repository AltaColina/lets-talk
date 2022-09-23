using LetsTalk.App.Models;
using LetsTalk.Commands.Auths;
using System.Collections.ObjectModel;

namespace LetsTalk.App.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ILetsTalkSettings _settings;
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHttpClient _httpClient;
    private readonly ILetsTalkHubClient _hubClient;
    private readonly ChatConnectionFactory _chatConnectionFactory;

    [ObservableProperty]
    private ObservableCollection<ChatConnection> _chatConnections = new();

    public MainViewModel(ILetsTalkSettings settings, INavigationService navigation, ILetsTalkHttpClient httpClient, ILetsTalkHubClient hubClient, ChatConnectionFactory chatConnectionFactory)
    {
        Title = "Let's Talk";
        _settings = settings;
        _navigation = navigation;
        _httpClient = httpClient;
        _hubClient = hubClient;
        _chatConnectionFactory = chatConnectionFactory;
    }

    [RelayCommand]
    private async Task OnLoadedAsync()
    {
        // Lets skip login for now.
        if (!_settings.IsAuthenticated)
        {
            _settings.Authentication = await _httpClient.LoginAsync(new LoginRequest
            {
                Username = "admin",
                Password = "super"
            });
        }

        if (_settings.IsAuthenticated)
        {
            if (!_hubClient.IsConnected)
                await _hubClient.ConnectAsync();

            var response = await _hubClient.GetUserChatsAsync();
            ChatConnections = new ObservableCollection<ChatConnection>(response.Chats.Select(c => _chatConnectionFactory.Create(c)));
        }
        else
        {
            await _navigation.GoToAsync<LoginViewModel>();
        }
    }

    [RelayCommand]
    private async Task OnChannelTappedAsync(ChatConnection connection)
    {
        await _navigation.GoToAsync<ChatViewModel>(new NavigationParameters { [nameof(ChatConnection)] = connection }, animate: false);
    }
}

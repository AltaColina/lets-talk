using CommunityToolkit.Maui.Views;
using LetsTalk.App.Models;
using LetsTalk.Security;
using Microsoft.AspNetCore.SignalR;
using System.Collections.ObjectModel;

namespace LetsTalk.App.ViewModels;

[QueryProperty(nameof(Authentication), nameof(Authentication))]
public partial class MainViewModel : BaseViewModel
{
    private readonly AppShell _appShell;
    private readonly ILetsTalkSettings _settings;
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHttpClient _httpClient;
    private readonly ILetsTalkHubClient _hubClient;
    private readonly ChatConnectionManager _chatConnectionManager;
    private Page? _view;

    public ObservableCollection<ChatConnection> ChatConnections { get => _chatConnectionManager.Connections; }

    [ObservableProperty]
    private bool _isChatListVisible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAuthenticated))]
    private Authentication? _authentication;

    public bool IsAuthenticated { get => _settings.IsAuthenticated; }

    public MainViewModel(AppShell appShell, ILetsTalkSettings settings, INavigationService navigation, ILetsTalkHttpClient httpClient, ILetsTalkHubClient hubClient, ChatConnectionManager chatConnectionManager)
    {
        Title = "Let's Talk";
        _appShell = appShell;
        _appShell.Window.BindingContext = this;
        _appShell.Window.SetBinding(Window.TitleProperty, new Binding(nameof(Title), BindingMode.TwoWay));
        _settings = settings;
        _navigation = navigation;
        _httpClient = httpClient;
        _hubClient = hubClient;
        _chatConnectionManager = chatConnectionManager;
    }

    [RelayCommand]
    private void OnNavigatedTo(Page page)
    {
        _view = page;
    }

    partial void OnAuthenticationChanged(Authentication? value)
    {
        _settings.Authentication = value;
        if (IsAuthenticated)
        {
            ConnectAsync().GetAwaiter();
        }
        else
        {
            ChatConnections.Clear();
        }

        async Task ConnectAsync()
        {
            try
            {
                if (_hubClient.IsConnected)
                    await _hubClient.DisconnectAsync();
                await _hubClient.ConnectAsync();
                var response = await _hubClient.GetUserChatsAsync();
                _chatConnectionManager.Reset(response.Chats);
                OnPropertyChanged(nameof(ChatConnections));
                Title = $"Let's Talk - {_settings.Authentication!.User.Name}";
                _view?.ShowPopup(new Popup { Content = new Label { Text = $"Connected as '{_settings.UserId}'" } });

            }
            catch (HubException ex)
            {
                _view?.ShowPopup(new Popup { Content = new Label { Text = ex.Message } });
            }
        }
    }

    [RelayCommand]
    private async Task OnSignInAsync() =>
        await _navigation.GoToAsync<LoginViewModel>();

    [RelayCommand]
    private async Task OnOpenChatAsync(ChatConnection connection) =>
        await _navigation.GoToAsync<ChatViewModel>(new NavigationParameters { [nameof(ChatConnection)] = connection }, animate: false);

    [RelayCommand]
    private async Task OnAddChatAsync() =>
        await _navigation.GoToAsync<AddChatViewModel>();
}

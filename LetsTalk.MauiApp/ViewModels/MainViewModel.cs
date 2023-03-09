using CommunityToolkit.Maui.Views;
using LetsTalk.Models;
using LetsTalk.Security;
using Microsoft.AspNetCore.SignalR;
using System.Collections.ObjectModel;

namespace LetsTalk.ViewModels;

[QueryProperty(nameof(Authentication), nameof(Authentication))]
public partial class MainViewModel : BaseViewModel
{
    private readonly AppShell _appShell;
    private readonly ILetsTalkSettings _settings;
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHttpClient _httpClient;
    private readonly ILetsTalkHubClient _hubClient;
    private readonly RoomConnectionManager _roomConnectionManager;
    private Page? _view;

    public ObservableCollection<RoomConnection> RoomConnections { get => _roomConnectionManager.Connections; }

    [ObservableProperty]
    private bool _isRoomListVisible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAuthenticated))]
    private Authentication? _authentication;

    public bool IsAuthenticated { get => _settings.IsAuthenticated; }

    public MainViewModel(AppShell appShell, ILetsTalkSettings settings, INavigationService navigation, ILetsTalkHttpClient httpClient, ILetsTalkHubClient hubClient, RoomConnectionManager roomConnectionManager)
    {
        Title = "Let's Talk";
        _appShell = appShell;
        _appShell.Window.BindingContext = this;
        _appShell.Window.SetBinding(Window.TitleProperty, new Binding(nameof(Title), BindingMode.TwoWay));
        _settings = settings;
        _navigation = navigation;
        _httpClient = httpClient;
        _hubClient = hubClient;
        _roomConnectionManager = roomConnectionManager;
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
            RoomConnections.Clear();
        }

        async Task ConnectAsync()
        {
            try
            {
                if (_hubClient.IsConnected)
                    await _hubClient.DisconnectAsync();
                await _hubClient.ConnectAsync();
                var response = await _hubClient.GetUserRoomsAsync();
                _roomConnectionManager.Reset(response.Rooms);
                OnPropertyChanged(nameof(RoomConnections));
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
    private async Task OnOpenRoomAsync(RoomConnection connection) =>
        await _navigation.GoToAsync<RoomViewModel>(new NavigationParameters { [nameof(RoomConnection)] = connection }, animate: false);

    [RelayCommand]
    private async Task OnAddRoomAsync() =>
        await _navigation.GoToAsync<AddRoomViewModel>();
}

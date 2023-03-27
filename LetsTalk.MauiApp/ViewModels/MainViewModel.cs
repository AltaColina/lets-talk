using LetsTalk.Models;
using System.Collections.ObjectModel;

namespace LetsTalk.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly AppShell _appShell;
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHttpClient _httpClient;
    private readonly ILetsTalkHubClient _hubClient;
    private readonly RoomConnectionManager _roomConnectionManager;
    private Page? _view;

    public ObservableCollection<RoomConnection> RoomConnections { get => _roomConnectionManager.Connections; }

    [ObservableProperty]
    private bool _isRoomListVisible;

    // TODO: Fix this to return the actually authentication state.
    [ObservableProperty]
    private bool _isAuthenticated;

    public MainViewModel(AppShell appShell, INavigationService navigation, ILetsTalkHttpClient httpClient, ILetsTalkHubClient hubClient, RoomConnectionManager roomConnectionManager)
    {
        Title = "Let's Talk";
        _appShell = appShell;
        _appShell.Window.BindingContext = this;
        _appShell.Window.SetBinding(Window.TitleProperty, new Binding(nameof(Title), BindingMode.TwoWay));
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

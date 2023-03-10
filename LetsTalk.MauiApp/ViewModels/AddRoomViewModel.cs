using LetsTalk.Rooms.Commands;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.ViewModels;
public sealed partial class AddRoomViewModel : BaseViewModel
{
    private readonly ILetsTalkSettings _settings;
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHttpClient _httpClient;
    private readonly ILetsTalkHubClient _hubClient;
    private readonly RoomConnectionManager _roomConnectionManager;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(JoinRoomCommand))]
    [NotifyCanExecuteChangedFor(nameof(CreateRoomCommand))]
    private string? _roomId;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(JoinRoomCommand))]
    [NotifyCanExecuteChangedFor(nameof(CreateRoomCommand))]
    private string? _roomName;

    [ObservableProperty]
    private bool _hasCreateRoomPermission;

    public bool CanJoinRoom { get => !String.IsNullOrWhiteSpace(RoomId); }
    public bool CanCreateRoom { get => HasCreateRoomPermission && !String.IsNullOrWhiteSpace(RoomId) && !String.IsNullOrWhiteSpace(RoomName); }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    public bool HasError { get => !String.IsNullOrWhiteSpace(ErrorMessage); }

    public AddRoomViewModel(ILetsTalkSettings settings, INavigationService navigation, ILetsTalkHttpClient httpClient, ILetsTalkHubClient hubClient, RoomConnectionManager roomConnectionManager)
    {
        _settings = settings;
        _navigation = navigation;
        _httpClient = httpClient;
        _hubClient = hubClient;
        _roomConnectionManager = roomConnectionManager;
    }

    [RelayCommand]
    private async Task OnNavigatedTo()
    {
        if (_settings.IsAuthenticated)
            HasCreateRoomPermission = _settings.Authentication.Permissions.Contains(Security.Permissions.Room.Create);
        else
            await _navigation.ReturnAsync();
    }

    [RelayCommand(CanExecute = nameof(CanJoinRoom))]
    private async Task OnJoinRoomAsync()
    {
        try
        {
            var response = await _hubClient.JoinRoomAsync(RoomId!);
            if (response.HasUserJoined)
            {
                _roomConnectionManager.Add(response.Room);
                await _navigation.ReturnAsync();
            }
        }
        catch (HubException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(CanCreateRoom))]
    private async Task OnCreateRoomAsync()
    {
        try
        {
            var room = await _httpClient.CreateRoomAsync(new CreateRoomCommand
            {
                Name = RoomName!,
            }, _settings.AccessToken!);
            RoomId = room.Id;
            var response = await _hubClient.JoinRoomAsync(RoomId!);
            if (response.HasUserJoined)
            {
                _roomConnectionManager.Add(response.Room);
                await _navigation.ReturnAsync();
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (HubException ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

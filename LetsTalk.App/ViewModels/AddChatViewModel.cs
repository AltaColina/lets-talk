using LetsTalk.Chats.Commands;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.App.ViewModels;
public sealed partial class AddChatViewModel : BaseViewModel
{
    private readonly ILetsTalkSettings _settings;
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHttpClient _httpClient;
    private readonly ILetsTalkHubClient _hubClient;
    private readonly ChatConnectionManager _chatConnectionManager;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(JoinChatCommand))]
    [NotifyCanExecuteChangedFor(nameof(CreateChatCommand))]
    private string? _chatId;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(JoinChatCommand))]
    [NotifyCanExecuteChangedFor(nameof(CreateChatCommand))]
    private string? _chatName;

    [ObservableProperty]
    private bool _hasCreateChatPermission;

    public bool CanJoinChat { get => !String.IsNullOrWhiteSpace(_chatId); }
    public bool CanCreateChat { get => HasCreateChatPermission && !String.IsNullOrWhiteSpace(_chatId) && !String.IsNullOrWhiteSpace(_chatName); }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    public bool HasError { get => !String.IsNullOrWhiteSpace(_errorMessage); }

    public AddChatViewModel(ILetsTalkSettings settings, INavigationService navigation, ILetsTalkHttpClient httpClient, ILetsTalkHubClient hubClient, ChatConnectionManager chatConnectionManager)
    {
        _settings = settings;
        _navigation = navigation;
        _httpClient = httpClient;
        _hubClient = hubClient;
        _chatConnectionManager = chatConnectionManager;
    }

    [RelayCommand]
    private async Task OnNavigatedTo()
    {
        if (_settings.IsAuthenticated)
            HasCreateChatPermission = _settings.Authentication.Permissions.Contains(Security.Permissions.Chat.Create);
        else
            await _navigation.ReturnAsync();
    }

    [RelayCommand(CanExecute = nameof(CanJoinChat))]
    private async Task OnJoinChatAsync()
    {
        try
        {
            var response = await _hubClient.JoinChatAsync(_chatId!);
            if (response.HasUserJoined)
            {
                _chatConnectionManager.Add(response.Chat);
                await _navigation.ReturnAsync();
            }
        }
        catch (HubException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(CanCreateChat))]
    private async Task OnCreateChatAsync()
    {
        try
        {
            var chatId = _chatId!;
            _ = await _httpClient.CreateChatAsync(new CreateChatCommand
            {
                Id = chatId,
                Name = _chatName!,
            }, _settings.AccessToken!);
            var response = await _hubClient.JoinChatAsync(_chatId!);
            if (response.HasUserJoined)
            {
                _chatConnectionManager.Add(response.Chat);
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

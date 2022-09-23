using LetsTalk.App.Models;

namespace LetsTalk.App.ViewModels;

[QueryProperty(nameof(ChatConnection), nameof(ChatConnection))]
public partial class ChatViewModel : BaseViewModel
{
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHubClient _letsTalkHubClient;

    [ObservableProperty]
    private ChatConnection _chatConnection = null!;

    [ObservableProperty]
    private string? _messageText;

    public MainViewModel MainViewModel { get; }

    public ChatViewModel(MainViewModel mainViewModel, INavigationService navigation, ILetsTalkHubClient letsTalkHubClient)
    {
        MainViewModel = mainViewModel;
        _navigation = navigation;
        _letsTalkHubClient = letsTalkHubClient;
    }

    [RelayCommand]
    private async Task OnNavigatedToAsync()
    {
        if (_chatConnection is not null)
        {
            Title = _chatConnection.Chat.Name;
            _chatConnection.IsChatVisible = true;
        }
        else
        {
            await _navigation.GoToAsync<MainViewModel>();
        }
    }

    [RelayCommand]
    private Task OnNavigatedFromAsync()
    {
        if (_chatConnection is not null)
            _chatConnection.IsChatVisible = false;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task OnSendMessageAsync()
    {
        if (!String.IsNullOrWhiteSpace(_messageText))
        {
            await _letsTalkHubClient.SendChatMessageAsync(_chatConnection.Chat.Id, _messageText);
            MessageText = null;
        }
    }
}

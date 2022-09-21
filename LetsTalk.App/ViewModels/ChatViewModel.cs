using LetsTalk.Models;
using System.Collections.ObjectModel;

namespace LetsTalk.App.ViewModels;

[QueryProperty(nameof(Chat), nameof(Chat))]
public partial class ChatViewModel : BaseViewModel
{
    private readonly ILetsTalkHubClient _letsTalkHubClient;

    [ObservableProperty]
    private ObservableCollection<ChatMessage> _messages = null!;

    [ObservableProperty]
    private Chat _chat = null!;

    [ObservableProperty]
    private string? _messageText;

    public ChatViewModel(INavigationService navigationService, ILetsTalkHubClient letsTalkHubClient)
        : base(navigationService)
    {
        _letsTalkHubClient = letsTalkHubClient;
    }

    [RelayCommand]
    private async Task OnAppearingAsync()
    {
        if (_chat is not null && _letsTalkHubClient.GetChatMessages(_chat.Id) is ObservableCollection<ChatMessage> messages)
        {
            await _letsTalkHubClient.JoinChatAsync(_chat.Id);
            Title = _chat.Id;
            Messages = messages;
        }
        else
        {
            await NavigationService.GoToAsync<MainViewModel>();
        }
    }

    [RelayCommand]
    private async Task OnDisappearingAsync()
    {
        if (_chat is not null)
            await _letsTalkHubClient.LeaveChatAsync(_chat.Id);
    }

    [RelayCommand]
    private async Task OnSendMessageAsync()
    {
        if (!String.IsNullOrWhiteSpace(_messageText))
        {
            await _letsTalkHubClient.SendChatMessageAsync(_chat.Id, _messageText);
            MessageText = null;
        }
    }
}

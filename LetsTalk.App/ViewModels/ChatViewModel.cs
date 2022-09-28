using LetsTalk.App.Models;
using LetsTalk.Messaging;
using System.Reflection;
using System.Text;

namespace LetsTalk.App.ViewModels;

[QueryProperty(nameof(ChatConnection), nameof(ChatConnection))]
public partial class ChatViewModel : BaseViewModel
{
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHubClient _letsTalkHubClient;

    [ObservableProperty]
    private ChatConnection _chatConnection = null!;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private string? _messageText;

    public bool CanSendMessage { get => !String.IsNullOrWhiteSpace(_messageText); }

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

    private static readonly IReadOnlyDictionary<string, string> CommandToContentTypeMap = typeof(MimeType.Image)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .ToDictionary(f => $"/{f.Name}", f => (string)f.GetValue(null)!, StringComparer.InvariantCultureIgnoreCase);

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task OnSendMessageAsync()
    {
        if (String.IsNullOrWhiteSpace(_messageText))
            throw new InvalidOperationException("Command configured incorrectly");

        if (_messageText[0] != '/')
        {
            await _letsTalkHubClient.SendChatMessageAsync(_chatConnection.Chat.Id, MimeType.Text.Plain, Encoding.UTF8.GetBytes(_messageText));
        }
        else if (_messageText.IndexOf(' ') is var index && index >= 0)
        {
            var command = _messageText[..index];
            var content = _messageText[(index + 1)..].Trim('"');
            if (!CommandToContentTypeMap.TryGetValue(command, out var contentType))
                return;
            await _letsTalkHubClient.SendChatMessageAsync(_chatConnection.Chat.Id, contentType, File.ReadAllBytes(content));
        }
        else
        {
            return;
        }
        MessageText = null;
    }
}

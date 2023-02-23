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

    public bool CanSendMessage { get => !String.IsNullOrWhiteSpace(MessageText); }

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
        if (ChatConnection is not null)
        {
            Title = ChatConnection.Chat.Name;
            ChatConnection.IsChatVisible = true;
        }
        else
        {
            await _navigation.GoToAsync<MainViewModel>();
        }
    }

    [RelayCommand]
    private Task OnNavigatedFromAsync()
    {
        if (ChatConnection is not null)
            ChatConnection.IsChatVisible = false;
        return Task.CompletedTask;
    }

    private static readonly IReadOnlyDictionary<string, string> CommandToContentTypeMap = typeof(MimeType.Image)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .ToDictionary(f => $"/{f.Name}", f => (string)f.GetValue(null)!, StringComparer.InvariantCultureIgnoreCase);

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task OnSendMessageAsync()
    {
        if (String.IsNullOrWhiteSpace(MessageText))
            throw new InvalidOperationException("Command configured incorrectly");

        if (MessageText[0] != '/')
        {
            await _letsTalkHubClient.SendChatMessageAsync(ChatConnection.Chat.Id, MimeType.Text.Plain, Encoding.UTF8.GetBytes(MessageText));
        }
        else if (MessageText.IndexOf(' ') is var index && index >= 0)
        {
            var command = MessageText[..index];
            var content = MessageText[(index + 1)..].Trim('"');
            if (!CommandToContentTypeMap.TryGetValue(command, out var contentType))
                return;
            await _letsTalkHubClient.SendChatMessageAsync(ChatConnection.Chat.Id, contentType, File.ReadAllBytes(content));
        }
        else
        {
            return;
        }
        MessageText = null;
    }
}

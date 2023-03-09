using LetsTalk.App.Models;
using LetsTalk.Messaging;
using System.Reflection;
using System.Text;

namespace LetsTalk.App.ViewModels;

[QueryProperty(nameof(RoomConnection), nameof(RoomConnection))]
public partial class RoomViewModel : BaseViewModel
{
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHubClient _letsTalkHubClient;

    [ObservableProperty]
    private RoomConnection _roomConnection = null!;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private string? _messageText;

    public bool CanSendMessage { get => !String.IsNullOrWhiteSpace(MessageText); }

    public MainViewModel MainViewModel { get; }

    public RoomViewModel(MainViewModel mainViewModel, INavigationService navigation, ILetsTalkHubClient letsTalkHubClient)
    {
        MainViewModel = mainViewModel;
        _navigation = navigation;
        _letsTalkHubClient = letsTalkHubClient;
    }

    [RelayCommand]
    private async Task OnNavigatedToAsync()
    {
        if (RoomConnection is not null)
        {
            Title = RoomConnection.Room.Name;
            RoomConnection.IsRoomVisible = true;
        }
        else
        {
            await _navigation.GoToAsync<MainViewModel>();
        }
    }

    [RelayCommand]
    private Task OnNavigatedFromAsync()
    {
        if (RoomConnection is not null)
            RoomConnection.IsRoomVisible = false;
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
            await _letsTalkHubClient.SendContentMessageAsync(RoomConnection.Room.Id, MimeType.Text.Plain, Encoding.UTF8.GetBytes(MessageText));
        }
        else if (MessageText.IndexOf(' ') is var index && index >= 0)
        {
            var command = MessageText[..index];
            var content = MessageText[(index + 1)..].Trim('"');
            if (!CommandToContentTypeMap.TryGetValue(command, out var contentType))
                return;
            await _letsTalkHubClient.SendContentMessageAsync(RoomConnection.Room.Id, contentType, File.ReadAllBytes(content));
        }
        else
        {
            return;
        }
        MessageText = null;
    }
}

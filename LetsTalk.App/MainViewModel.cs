using LetsTalk.Models;
using LetsTalk.Models.Auths;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LetsTalk.App;

[QueryProperty(nameof(Authentication), nameof(Authentication))]
public partial class MainViewModel : BaseViewModel
{
    private readonly ILetsTalkHttpClient _letsTalkClient;
    private readonly ILetsTalkHubClient _letsTalkHub;

    public AuthenticationResponse? Authentication { get; set; }

    public ObservableCollection<Chat> Chats { get; } = new();

    public MainViewModel(INavigationService navigationService, ILetsTalkHttpClient letsTalkClient, ILetsTalkHubClient letsTalkHub)
        : base(navigationService)
    {
        Title = "Let's Talk";
        _letsTalkClient = letsTalkClient;
        _letsTalkHub = letsTalkHub;
    }

    private async Task<string?> ProvideToken()
    {
        if (Authentication is null)
            return null;
        if (Authentication.AccessToken.ExpiresIn < DateTimeOffset.UtcNow)
        {
            try
            {
                Authentication = await _letsTalkClient.RefreshAsync(new RefreshRequest
                {
                    Username = Authentication.Person.Username,
                    RefreshToken = Authentication.RefreshToken.Id
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }
        return Authentication?.AccessToken.Id;
    }

    [RelayCommand]
    private async Task OnAppearing()
    {
        var token = await ProvideToken();
        if (token is not null)
        {
            if (!_letsTalkHub.IsConnected)
                await _letsTalkHub.ConnectAsync("https://localhost:7219/letsTalk", ProvideToken);
            if (Chats.Count > 0)
                Chats.Clear();
            var response = await _letsTalkClient.ChatGetAsync(token);
            foreach (var chat in response.Chats)
                Chats.Add(chat);
        }
        else
        {
            await NavigationService.GoToAsync(nameof(LoginPage));
        }
    }

    [RelayCommand]
    private async Task OnChannelTapped(Chat? chat)
    {
        if (chat is null)
            return;

        await NavigationService.GoToAsync(nameof(ChatPage), new NavigationParameters
        {
            [nameof(Chat)] = chat
        });
    }
}

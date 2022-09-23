using LetsTalk.Commands.Auths;
using LetsTalk.Dtos;
using LetsTalk.Models;
using System.Collections.ObjectModel;

namespace LetsTalk.App.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ILetsTalkHttpClient _httpClient;
    private readonly ILetsTalkHubClient _hubClient;

    public ObservableCollection<ChatDto> Chats { get; } = new();

    public MainViewModel(ILetsTalkHttpClient httpClient, ILetsTalkHubClient hubClient)
    {
        Title = "Let's Talk";
        _httpClient = httpClient;
        _hubClient = hubClient;
    }

    //private async Task<string?> ProvideToken()
    //{
    //    if (Authentication is null)
    //        return null;
    //    if (Authentication.AccessToken.ExpiresIn < DateTimeOffset.UtcNow)
    //    {
    //        try
    //        {
    //            Authentication = await _letsTalkClient.RefreshAsync(new RefreshRequest
    //            {
    //                Username = Authentication.User.Id,
    //                RefreshToken = Authentication.RefreshToken.Id
    //            });
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.WriteLine(ex);
    //            throw;
    //        }
    //    }
    //    return Authentication?.AccessToken.Id;
    //}

    [RelayCommand]
    private async Task OnAppearing()
    {
        // Lets skip login for now.
        if (!Settings.IsAuthenticated)
        {
            Settings.Authentication = await _httpClient.LoginAsync(new LoginRequest
            {
                Username = "admin",
                Password = "super"
            });
        }

        if (Settings.IsAuthenticated)
        {
            if (!_hubClient.IsConnected)
                await _hubClient.ConnectAsync();

            var chats = await _httpClient.GetUserChatsAsync(Settings.UserId, Settings.AccessToken);
        }
        else
        {
            await Navigation.GoToAsync<LoginViewModel>();
        }
    }

    [RelayCommand]
    private async Task OnChannelTapped(Chat? chat)
    {
        if (chat is null)
            return;

        await Navigation.GoToAsync<ChatViewModel>(new NavigationParameters
        {
            [nameof(Chat)] = chat
        });
    }
}

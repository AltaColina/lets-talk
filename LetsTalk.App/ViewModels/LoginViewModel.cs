using LetsTalk.Dtos.Auths;
using System.Diagnostics;

namespace LetsTalk.App.ViewModels;
public partial class LoginViewModel : BaseViewModel
{
    private readonly ILetsTalkHttpClient _letsTalkClient;

    [ObservableProperty]
	private string? _loginUsername;
    
    [ObservableProperty]
    private string? _loginPassword;
    
    [ObservableProperty]
    private string? _registerUsername;
    
    [ObservableProperty]
    private string? _registerPassword;

    public LoginViewModel(INavigationService navigationService, ILetsTalkHttpClient letsTalkClient)
		: base(navigationService)
	{
		Title = "Login";
        _letsTalkClient = letsTalkClient;
    }

    [RelayCommand]
    private async Task OnLoginAsync()
    {
        try
        {
            var response = await _letsTalkClient.LoginAsync(new LoginRequest
            {
                Username = LoginUsername!,
                Password = LoginPassword!
            });
            await NavigationService.GoToAsync<MainViewModel>(new NavigationParameters
            {
                ["Authentication"] = response
            });
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine(ex);
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    [RelayCommand]
    private async Task OnRegisterAsync()
    {
        try
        {
            var response = await _letsTalkClient.RegisterAsync(new RegisterRequest
            {
                Username = LoginUsername!,
                Password = LoginPassword!
            });
            await NavigationService.GoToAsync<MainViewModel>(new NavigationParameters
            {
                ["Authentication"] = response
            });
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine(ex);
        }
    }
}

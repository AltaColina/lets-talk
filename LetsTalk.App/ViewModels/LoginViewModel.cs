using LetsTalk.Security.Commands;

namespace LetsTalk.App.ViewModels;
public partial class LoginViewModel : BaseViewModel
{
    private readonly INavigationService _navigation;
    private readonly ILetsTalkHttpClient _letsTalkClient;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string? _loginUsername;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string? _loginPassword;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string? _registerUsername;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string? _registerPassword;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string? _registerDisplayName;

    public bool CanLogin { get => !String.IsNullOrWhiteSpace(_loginUsername) && !String.IsNullOrWhiteSpace(_loginPassword); }

    public bool CanRegister { get => !String.IsNullOrWhiteSpace(_registerUsername) && !String.IsNullOrWhiteSpace(_registerPassword) && (_registerDisplayName is null || !String.IsNullOrWhiteSpace(_registerDisplayName)); }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    private bool HasError { get => !String.IsNullOrWhiteSpace(_errorMessage); }

    public LoginViewModel(INavigationService navigation, ILetsTalkHttpClient letsTalkClient)
    {
        Title = "Login";
        _navigation = navigation;
        _letsTalkClient = letsTalkClient;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task OnLoginAsync()
    {
        try
        {
            var response = await _letsTalkClient.LoginAsync(new LoginCommand
            {
                Username = LoginUsername!,
                Password = LoginPassword!
            });
            await _navigation.ReturnAsync(new NavigationParameters
            {
                ["Authentication"] = response
            });
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private async Task OnRegisterAsync()
    {
        try
        {
            var response = await _letsTalkClient.RegisterAsync(new RegisterCommand
            {
                Username = RegisterUsername!,
                Password = RegisterPassword!,
                Name = RegisterDisplayName
            });
            await _navigation.ReturnAsync(new NavigationParameters
            {
                ["Authentication"] = response
            });
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

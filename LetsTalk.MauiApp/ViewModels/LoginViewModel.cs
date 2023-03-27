namespace LetsTalk.ViewModels;
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

    public bool CanLogin { get => !String.IsNullOrWhiteSpace(LoginUsername) && !String.IsNullOrWhiteSpace(LoginPassword); }

    public bool CanRegister { get => !String.IsNullOrWhiteSpace(RegisterUsername) && !String.IsNullOrWhiteSpace(RegisterPassword); }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    private bool HasError { get => !String.IsNullOrWhiteSpace(ErrorMessage); }

    public LoginViewModel(INavigationService navigation, ILetsTalkHttpClient letsTalkClient)
    {
        Title = "Login";
        _navigation = navigation;
        _letsTalkClient = letsTalkClient;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private Task OnLoginAsync()
    {
        // TODO: Perform login here.
        //try
        //{
        //    var response = await _letsTalkClient.LoginAsync(LoginUsername!, LoginPassword!);
        //    await _navigation.ReturnAsync(new NavigationParameters
        //    {
        //        ["Authentication"] = response
        //    });
        //}
        //catch (HttpRequestException ex)
        //{
        //    ErrorMessage = ex.Message;
        //}
        return Task.CompletedTask;
    }

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private Task OnRegisterAsync()
    {
        // TODO: Perform register here.
        //try
        //{
        //    var response = await _letsTalkClient.CreateUserAsync(RegisterUsername!, RegisterPassword!);
        //    await _navigation.ReturnAsync(new NavigationParameters
        //    {
        //        ["Authentication"] = response
        //    });
        //}
        //catch (HttpRequestException ex)
        //{
        //    ErrorMessage = ex.Message;
        //}
        return Task.CompletedTask;
    }
}

namespace LetsTalk.App.ViewModels;
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _title;

    //protected INavigationService Navigation { get; } = App.Current.Services.GetRequiredService<INavigationService>();

    //protected ILetsTalkSettings Settings { get; } = App.Current.Services.GetRequiredService<ILetsTalkSettings>();

    protected BaseViewModel()
    {
        _title = GetType().Name.Replace("ViewModel", "");
    }
}

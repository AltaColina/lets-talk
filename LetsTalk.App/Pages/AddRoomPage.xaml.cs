namespace LetsTalk.App.Pages;

public partial class AddRoomPage : ContentPage
{
    public AddRoomPage(AddRoomViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
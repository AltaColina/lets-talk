namespace LetsTalk.Pages;

public partial class AddRoomPage : ContentPage
{
    public AddRoomPage(AddRoomViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
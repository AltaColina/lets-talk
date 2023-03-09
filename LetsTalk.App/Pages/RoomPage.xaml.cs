namespace LetsTalk.App.Pages;

public partial class RoomPage : ContentPage
{
	public RoomPage(RoomViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}
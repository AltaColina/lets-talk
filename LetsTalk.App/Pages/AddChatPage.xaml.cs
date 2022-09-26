namespace LetsTalk.App.Pages;

public partial class AddChatPage : ContentPage
{
	public AddChatPage(AddChatViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
namespace LetsTalk.App.Pages;

public partial class ChatPage : ContentPage
{
	public ChatPage(ChatViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
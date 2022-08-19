namespace LetsTalk.App.Views;

public partial class ChatPage : ContentPage
{
	public ChatPage(ChatViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
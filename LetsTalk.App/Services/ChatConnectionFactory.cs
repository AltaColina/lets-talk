using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.App.Models;
using LetsTalk.Dtos;

namespace LetsTalk.App.Services;
public sealed class ChatConnectionFactory
{
	private readonly IMessenger _messenger;

	public ChatConnectionFactory(IMessenger messenger)
	{
		_messenger = messenger;
	}

	public ChatConnection Create(ChatDto chat) => new(_messenger, chat);
}

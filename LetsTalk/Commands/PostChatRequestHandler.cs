using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class PostChatRequestHandler : IRequestHandler<ChatPostRequest, ChatPostResponse>
{
    private readonly IChatRepository _chatRepository;

    public PostChatRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }
    
    public async Task<ChatPostResponse> Handle(ChatPostRequest request, CancellationToken cancellationToken)
    {
        var chat = new Chat { Id = Guid.NewGuid().ToString(), Name = request.Name };
        try
        {
            await _chatRepository.InsertAsync(chat);
        }
        catch(Exception ex)
        {
            throw new UnknownException(ex.Message);
        }

        return new ChatPostResponse { Chat = chat };
    }
}

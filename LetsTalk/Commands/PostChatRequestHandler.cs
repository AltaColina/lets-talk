using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class PostChatRequestHandler : IRequestHandler<PostChatRequest, PostChatResponse>
{
    private readonly IChatRepository _chatRepository;

    public PostChatRequestHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }
    
    public async Task<PostChatResponse> Handle(PostChatRequest request, CancellationToken cancellationToken)
    {
        var chat = new ChatInfo { Id = Guid.NewGuid().ToString(), Name = request.Name };
        try
        {
            await _chatRepository.InsertAsync(new ChatRoom { Id = Guid.NewGuid().ToString(), Name = request.Name });
        }
        catch(Exception ex)
        {
            throw new UnknownException(ex.Message);
        }

        return new PostChatResponse { Chat = chat };
    }
}

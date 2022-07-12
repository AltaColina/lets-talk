using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Models.Chats;
using MediatR;

namespace LetsTalk.Commands;

public sealed class ChatRequestHandler : IRequestHandler<ChatGetRequest, ChatGetResponse>,
                                         IRequestHandler<ChatPostRequest>,
                                         IRequestHandler<ChatPutRequest>,
                                         IRequestHandler<ChatDeleteRequest>,
                                         IRequestHandler<ChatUserGetRequest, ChatUserGetResponse>,
                                         IRequestHandler<ChatUserPutRequest>
{
    private readonly IRepository<Chat> _chatRepository;
    private readonly IRepository<User> _userRepository;
    public ChatRequestHandler(IRepository<Chat> chatRepository, IRepository<User> userRepository)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }

    public async Task<ChatGetResponse> Handle(ChatGetRequest request, CancellationToken cancellationToken)
    {
        if (request.ChatId is null)
            return new ChatGetResponse { Chats = new List<Chat>(await _chatRepository.GetAllAsync()) };

        var chat = await _chatRepository.GetAsync(request.ChatId);
        if (chat is null)
            throw new NotFoundException($"Chat {request.ChatId} does not exist");
        return new ChatGetResponse { Chats = { chat } };
    }

    public async Task<Unit> Handle(ChatPostRequest request, CancellationToken cancellationToken)
    {
        var role = await _chatRepository.GetAsync(request.Chat.Id);
        if (role is not null)
            throw new ConflictException($"Role {request.Chat.Id} already exists");
        await _chatRepository.InsertAsync(request.Chat);
        return Unit.Value;
    }

    public async Task<Unit> Handle(ChatPutRequest request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetAsync(request.Chat.Id);
        if (chat is null)
            throw new NotFoundException($"Chat {request.Chat.Id} does not exist");
        await _chatRepository.UpdateAsync(chat);
        return Unit.Value;
    }

    public async Task<Unit> Handle(ChatDeleteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _chatRepository.DeleteAsync(request.ChatId);
        }
        catch (Exception ex)
        {
            throw new NotFoundException(ex.Message);
        }

        return Unit.Value;
    }

    public async Task<ChatUserGetResponse> Handle(ChatUserGetRequest request, CancellationToken cancellationToken)
    {
        var chat = _chatRepository.GetAsync(request.ChatId);
        if (chat is null)
            throw new NotFoundException($"Chat {request.ChatId} does not exist");
        var users = (await _userRepository.GetAllAsync()).Where(user => user.Chats.Contains(request.ChatId));
        return new ChatUserGetResponse { Users = new(users) };
    }

    public async Task<Unit> Handle(ChatUserPutRequest request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetAsync(request.ChatId);
        if (chat is null)
            throw new NotFoundException($"Chat {request.ChatId} does not exist");
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");

        user.Chats.Add(chat.Id);
        await _chatRepository.UpdateAsync(chat);
        return Unit.Value;
    }
}

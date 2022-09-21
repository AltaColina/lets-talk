using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Dtos.Chats;
using LetsTalk.Dtos.Users;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class ChatRequestHandler : IRequestHandler<GetChatsRequest, GetChatsResponse>,
                                         IRequestHandler<CreateChatRequest, ChatDto>,
                                         IRequestHandler<UpdateChatRequest>,
                                         IRequestHandler<DeleteChatRequest>,
                                         IRequestHandler<GetChatUsersRequest, GetChatUsersResponse>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Chat> _chatRepository;
    private readonly IRepository<User> _userRepository;

    public ChatRequestHandler(IMapper mapper, IRepository<Chat> chatRepository, IRepository<User> userRepository)
    {
        _mapper = mapper;
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }

    public async Task<GetChatsResponse> Handle(GetChatsRequest request, CancellationToken cancellationToken)
    {
        if (request.Id is null)
            return new GetChatsResponse { Chats = _mapper.Map<List<ChatDto>>(await _chatRepository.ListAsync(cancellationToken)) };

        var chat = await _chatRepository.GetByIdAsync(request.Id, cancellationToken);
        if (chat is null)
            throw new NotFoundException($"Chat {request.Id} does not exist");
        return new GetChatsResponse { Chats = { _mapper.Map<ChatDto>(chat) } };
    }

    public async Task<ChatDto> Handle(CreateChatRequest request, CancellationToken cancellationToken)
    {
        if ((await _chatRepository.GetByIdAsync(request.Id, cancellationToken)) is not null)
            throw new ConflictException($"Chat {request.Id} already exists");
        var chat = _mapper.Map<Chat>(request);
        chat.Name ??= chat.Id;
        await _chatRepository.AddAsync(chat, cancellationToken);
        return _mapper.Map<ChatDto>(chat);
    }

    public async Task<Unit> Handle(UpdateChatRequest request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetByIdAsync(request.Id, cancellationToken);
        if (chat is null)
            throw new NotFoundException($"Chat {request.Id} does not exist");
        chat = _mapper.Map(request, chat);
        await _chatRepository.UpdateAsync(chat, cancellationToken);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteChatRequest request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetByIdAsync(request.Id, cancellationToken);
        if (chat is null)
            throw new NotFoundException($"Chat {request.Id} does not exist");
        await _chatRepository.DeleteAsync(chat, cancellationToken);

        return Unit.Value;
    }

    private sealed class GetChatUsersRequestSpecification : Specification<User>
    {
        public GetChatUsersRequestSpecification(string chatId)
        {
            Query.Where(user => user.Chats.Contains(chatId));
        }
    }

    public async Task<GetChatUsersResponse> Handle(GetChatUsersRequest request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetByIdAsync(request.Id, cancellationToken);
        if (chat is null)
            throw new NotFoundException($"Chat {request.Id} does not exist");

        var users = await _userRepository.ListAsync(new GetChatUsersRequestSpecification(request.Id), cancellationToken);
        return new GetChatUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
    }
}

using AutoMapper;
using FluentValidation;
using LetsTalk.Chats;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Users;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Hubs.Commands;

public sealed class LeaveChatResponse
{
    [MemberNotNullWhen(true, nameof(Chat))]
    public bool HasUserLeft { get; init; }
    public UserDto User { get; init; } = null!;
    public ChatDto? Chat { get; init; }
}

public sealed class LeaveChatCommand : IRequest<LeaveChatResponse>
{
    public string ChatId { get; init; } = null!;
    public string UserId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<JoinChatCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.ChatId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<LeaveChatCommand, LeaveChatResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IMapper mapper, IRepository<User> userRepository, IRepository<Chat> chatRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<LeaveChatResponse> Handle(LeaveChatCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new UnauthorizedException($"Invalid user '{request.UserId}'");

            if (!user.Chats.Contains(request.ChatId))
                return new LeaveChatResponse { User = _mapper.Map<UserDto>(user) };

            var chat = await _chatRepository.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.ChatId} does not exist");

            user.Chats.Remove(request.ChatId);
            chat.Users.Remove(request.UserId);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _chatRepository.UpdateAsync(chat, cancellationToken);

            return new LeaveChatResponse
            {
                HasUserLeft = true,
                User = _mapper.Map<UserDto>(user),
                Chat = _mapper.Map<ChatDto>(chat),
            };
        }
    }
}

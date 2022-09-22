using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Commands.Hubs;

public sealed class JoinChatResponse
{
    [MemberNotNullWhen(true, nameof(Chat))]
    public bool HasUserJoined { get; init; }
    public User User { get; init; } = null!;
    public Chat? Chat { get; init; }
}

public sealed class JoinChatRequest : IRequest<JoinChatResponse>
{
    public string UserId { get; init; } = null!;
    public string ChatId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<JoinChatRequest>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.ChatId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<JoinChatRequest, JoinChatResponse>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IRepository<User> userRepository, IRepository<Chat> chatRepository)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<JoinChatResponse> Handle(JoinChatRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new UnauthorizedException($"Invalid user '{request.UserId}'");

            if (user.Chats.Contains(request.ChatId))
                return new JoinChatResponse { User = user };

            var chat = await _chatRepository.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.ChatId} does not exist");

            user.Chats.Add(request.ChatId);
            chat.Users.Add(request.UserId);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _chatRepository.UpdateAsync(chat, cancellationToken);

            return new JoinChatResponse
            {
                HasUserJoined = true,
                User = user,
                Chat = chat,
            };
        }
    }
}

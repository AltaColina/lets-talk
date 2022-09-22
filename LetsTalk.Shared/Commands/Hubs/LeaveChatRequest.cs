﻿using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LetsTalk.Commands.Hubs;

public sealed class LeaveChatResponse
{
    [MemberNotNullWhen(true, nameof(Chat))]
    public bool HasUserLeft { get; init; }
    public User User { get; init; } = null!;
    public Chat? Chat { get; init; }
}

public sealed class LeaveChatRequest : IRequest<LeaveChatResponse>
{
    public string ChatId { get; init; } = null!;
    public string UserId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<JoinChatRequest>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.ChatId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<LeaveChatRequest, LeaveChatResponse>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IRepository<User> userRepository, IRepository<Chat> chatRepository)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<LeaveChatResponse> Handle(LeaveChatRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new UnauthorizedException($"Invalid user '{request.UserId}'");

            if (!user.Chats.Contains(request.ChatId))
                return new LeaveChatResponse { User = user };

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
                User = user,
                Chat = chat,
            };
        }
    }
}

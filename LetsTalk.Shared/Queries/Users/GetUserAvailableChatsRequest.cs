﻿using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Queries.Chats;

public sealed class GetUserAvailableChatsResponse
{
    public List<ChatDto> Chats { get; init; } = null!;
}

public sealed class GetUserAvailableChatsRequest : IRequest<GetUserAvailableChatsResponse>
{
    public string UserId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetUserAvailableChatsRequest>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
        }
    }

    public sealed class Specification : Specification<Chat>
    {
        public Specification(ICollection<string> chatIds)
        {
            Query.Where(chat => !chatIds.Contains(chat.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetUserAvailableChatsRequest, GetUserAvailableChatsResponse>
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

        public async Task<GetUserAvailableChatsResponse> Handle(GetUserAvailableChatsRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new NotFoundException($"User {request.UserId} does not exist");

            var chats = await _chatRepository.ListAsync(new Specification(user.Chats), cancellationToken);
            return new GetUserAvailableChatsResponse { Chats = _mapper.Map<List<ChatDto>>(chats) };
        }
    }
}

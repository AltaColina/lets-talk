﻿using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Chats.Queries;

public sealed class GetChatUsersResponse
{
    public List<UserDto> Users { get; init; } = null!;
}

public sealed class GetChatUsersQuery : IRequest<GetChatUsersResponse>
{
    public string ChatId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetChatUsersQuery>
    {
        public Validator()
        {
            RuleFor(e => e.ChatId).NotEmpty();
        }
    }

    public sealed class Specification : Specification<User>
    {
        public Specification(ICollection<string> userIds)
        {
            Query.Where(user => userIds.Contains(user.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetChatUsersQuery, GetChatUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public Handler(IMapper mapper, IChatRepository chatRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<GetChatUsersResponse> Handle(GetChatUsersQuery request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.ChatId} does not exist");

            var users = await _userRepository.ListAsync(new Specification(chat.Users), cancellationToken);
            return new GetChatUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

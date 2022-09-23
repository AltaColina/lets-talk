using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Queries.Chats;

public sealed class GetChatUsersResponse
{
    public List<UserDto> Users { get; init; } = null!;
}

public sealed class GetChatUsersRequest : IRequest<GetChatUsersResponse>
{
    public string ChatId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetChatUsersRequest>
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

    public sealed class Handler : IRequestHandler<GetChatUsersRequest, GetChatUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<User> _userRepository;

        public Handler(IMapper mapper, IRepository<Chat> chatRepository, IRepository<User> userRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<GetChatUsersResponse> Handle(GetChatUsersRequest request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.ChatId} does not exist");

            var users = await _userRepository.ListAsync(new Specification(chat.Users), cancellationToken);
            return new GetChatUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

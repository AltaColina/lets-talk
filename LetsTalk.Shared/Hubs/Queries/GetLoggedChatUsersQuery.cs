using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Chats;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Hubs.Queries;

public sealed class GetLoggedChatUsersResponse
{
    public List<UserDto> Users { get; init; } = null!;
}

public sealed class GetLoggedChatUsersQuery : IRequest<GetLoggedChatUsersResponse>
{
    public string ChatId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetLoggedChatUsersQuery>
    {
        public Validator()
        {
            RuleFor(e => e.ChatId).NotEmpty();
        }
    }

    public sealed class Specification : Specification<User>
    {
        public Specification(IReadOnlyCollection<string> userIds)
        {
            Query.Where(user => userIds.Contains(user.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetLoggedChatUsersQuery, GetLoggedChatUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IMapper mapper, IHubConnectionManager connectionManager, IRepository<User> userRepository, IRepository<Chat> chatRepository)
        {
            _mapper = mapper;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<GetLoggedChatUsersResponse> Handle(GetLoggedChatUsersQuery request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.ChatId} does not exist");
            var userIds = _connectionManager.GetUserIds().Intersect(chat.Users).ToList();
            var users = await _userRepository.ListAsync(new Specification(userIds), cancellationToken);
            return new GetLoggedChatUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Chats;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Users.Queries;

public sealed class GetUserAvailableChatsResponse
{
    public List<ChatDto> Chats { get; init; } = null!;
}

public sealed class GetUserAvailableChatsQuery : IRequest<GetUserAvailableChatsResponse>
{
    public string UserId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetUserAvailableChatsQuery>
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

    public sealed class Handler : IRequestHandler<GetUserAvailableChatsQuery, GetUserAvailableChatsResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;

        public Handler(IMapper mapper, IUserRepository userRepository, IChatRepository chatRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<GetUserAvailableChatsResponse> Handle(GetUserAvailableChatsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new NotFoundException($"User {request.UserId} does not exist");

            var chats = await _chatRepository.ListAsync(new Specification(user.Chats), cancellationToken);
            return new GetUserAvailableChatsResponse { Chats = _mapper.Map<List<ChatDto>>(chats) };
        }
    }
}

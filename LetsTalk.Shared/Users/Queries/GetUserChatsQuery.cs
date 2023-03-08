using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Chats;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Users.Queries;

public sealed class GetUserChatsResponse
{
    public List<ChatDto> Chats { get; init; } = null!;
}

public sealed class GetUserChatsQuery : IRequest<GetUserChatsResponse>
{
    public string UserId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetUserChatsQuery>
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
            Query.Where(chat => chatIds.Contains(chat.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetUserChatsQuery, GetUserChatsResponse>
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

        public async Task<GetUserChatsResponse> Handle(GetUserChatsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new NotFoundException($"User {request.UserId} does not exist");

            var chats = await _chatRepository.ListAsync(new Specification(user.Chats), cancellationToken);
            return new GetUserChatsResponse { Chats = _mapper.Map<List<ChatDto>>(chats) };
        }
    }
}

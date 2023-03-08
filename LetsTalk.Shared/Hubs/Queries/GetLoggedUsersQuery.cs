using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Hubs.Queries;

public sealed class GetLoggedUsersResponse
{
    public List<UserDto> Users { get; init; } = null!;
}

public sealed class GetLoggedUsersQuery : IRequest<GetLoggedUsersResponse>
{
    public sealed class Specification : Specification<User>
    {
        public Specification(IReadOnlyCollection<string> userIds)
        {
            Query.Where(user => userIds.Contains(user.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetLoggedUsersQuery, GetLoggedUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IUserRepository _userRepository;

        public Handler(IMapper mapper, IHubConnectionManager connectionManager, IUserRepository userRepository)
        {
            _mapper = mapper;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
        }

        public async Task<GetLoggedUsersResponse> Handle(GetLoggedUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.ListAsync(new Specification(_connectionManager.GetUserIds()), cancellationToken);
            return new GetLoggedUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

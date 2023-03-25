using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Repositories;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Hubs.Queries;

public sealed class GetUsersLoggedInResponse
{
    public required List<UserDto> Users { get; init; }
}

public sealed class GetUsersLoggedInQuery : IRequest<GetUsersLoggedInResponse>
{
    public sealed class Specification : Specification<User>
    {
        public Specification(IReadOnlyCollection<string> userIds)
        {
            Query.Where(user => userIds.Contains(user.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetUsersLoggedInQuery, GetUsersLoggedInResponse>
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

        public async Task<GetUsersLoggedInResponse> Handle(GetUsersLoggedInQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.ListAsync(new Specification(_connectionManager.GetUserIds()), cancellationToken);
            return new GetUsersLoggedInResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

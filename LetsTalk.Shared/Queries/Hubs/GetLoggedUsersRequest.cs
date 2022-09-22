using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Dtos;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Queries.Hubs;

public sealed class GetLoggedUsersResponse
{
    public List<UserDto> Users { get; init; } = null!;
}

public sealed class GetLoggedUsersRequest : IRequest<GetLoggedUsersResponse>
{
    public sealed class Specification : Specification<User>
    {
        public Specification(IReadOnlyCollection<string> userIds)
        {
            Query.Where(user => userIds.Contains(user.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetLoggedUsersRequest, GetLoggedUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IRepository<User> _userRepository;

        public Handler(IMapper mapper, IHubConnectionManager connectionManager, IRepository<User> userRepository)
        {
            _mapper = mapper;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
        }

        public async Task<GetLoggedUsersResponse> Handle(GetLoggedUsersRequest request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.ListAsync(new Specification(_connectionManager.GetUserIds()), cancellationToken);
            return new GetLoggedUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

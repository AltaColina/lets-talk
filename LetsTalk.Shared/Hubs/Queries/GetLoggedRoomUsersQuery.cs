using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Hubs.Queries;

public sealed class GetLoggedRoomUsersResponse
{
    public List<UserDto> Users { get; init; } = null!;
}

public sealed class GetLoggedRoomUsersQuery : IRequest<GetLoggedRoomUsersResponse>
{
    public string RoomId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetLoggedRoomUsersQuery>
    {
        public Validator()
        {
            RuleFor(e => e.RoomId).NotEmpty();
        }
    }

    public sealed class Specification : Specification<User>
    {
        public Specification(IReadOnlyCollection<string> userIds)
        {
            Query.Where(user => userIds.Contains(user.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetLoggedRoomUsersQuery, GetLoggedRoomUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IHubConnectionManager connectionManager, IUserRepository userRepository, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<GetLoggedRoomUsersResponse> Handle(GetLoggedRoomUsersQuery request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
                throw new NotFoundException($"Room {request.RoomId} does not exist");
            var userIds = _connectionManager.GetUserIds().Intersect(room.Users).ToList();
            var users = await _userRepository.ListAsync(new Specification(userIds), cancellationToken);
            return new GetLoggedRoomUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

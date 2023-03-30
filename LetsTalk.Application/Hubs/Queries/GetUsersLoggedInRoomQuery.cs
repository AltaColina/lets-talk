using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using LetsTalk.Rooms;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Hubs.Queries;

public sealed class GetUsersLoggedInRoomResponse
{
    public required List<UserDto> Users { get; init; }
}

public sealed class GetUsersLoggedInRoomQuery : IRequest<GetUsersLoggedInRoomResponse>
{
    public required string RoomId { get; init; }

    public sealed class Validator : AbstractValidator<GetUsersLoggedInRoomQuery>
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

    public sealed class Handler : IRequestHandler<GetUsersLoggedInRoomQuery, GetUsersLoggedInRoomResponse>
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

        public async Task<GetUsersLoggedInRoomResponse> Handle(GetUsersLoggedInRoomQuery request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken) ?? throw ExceptionFor<Room>.NotFound(r => r.Id, request.RoomId);
            var userIds = _connectionManager.GetUserIds().Intersect(room.Users).ToList();
            var users = await _userRepository.ListAsync(new Specification(userIds), cancellationToken);
            return new GetUsersLoggedInRoomResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

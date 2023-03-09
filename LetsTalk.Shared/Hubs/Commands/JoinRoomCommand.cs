using AutoMapper;
using FluentValidation;
using LetsTalk.Rooms;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using LetsTalk.Users;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Hubs.Commands;

public sealed class JoinRoomResponse
{
    [MemberNotNullWhen(true, nameof(Room))]
    public bool HasUserJoined { get; init; }
    public UserDto User { get; init; } = null!;
    public RoomDto? Room { get; init; }
}

public sealed class JoinRoomCommand : IRequest<JoinRoomResponse>
{
    public string UserId { get; init; } = null!;
    public string RoomId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<JoinRoomCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.RoomId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<JoinRoomCommand, JoinRoomResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IUserRepository userRepository, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<JoinRoomResponse> Handle(JoinRoomCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new UnauthorizedException($"Invalid user '{request.UserId}'");

            if (user.Rooms.Contains(request.RoomId))
                return new JoinRoomResponse { User = _mapper.Map<UserDto>(user) };

            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
                throw new NotFoundException($"Room {request.RoomId} does not exist");

            user.Rooms.Add(request.RoomId);
            room.Users.Add(request.UserId);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _roomRepository.UpdateAsync(room, cancellationToken);

            return new JoinRoomResponse
            {
                HasUserJoined = true,
                User = _mapper.Map<UserDto>(user),
                Room = _mapper.Map<RoomDto>(room),
            };
        }
    }
}

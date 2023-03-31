using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Rooms;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;
using OneOf.Types;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Hubs.Commands;

public sealed class JoinRoomResponse
{
    [MemberNotNullWhen(true, nameof(Room))]
    public bool HasUserJoined { get => Room is not null; }
    public required User User { get; init; }
    public required Room? Room { get; init; }
}

public sealed class JoinRoomCommand : IRequest<Response<JoinRoomResponse>>
{
    public required string UserId { get; init; }
    public required string RoomId { get; init; }

    public sealed class Validator : AbstractValidator<JoinRoomCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.RoomId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<JoinRoomCommand, Response<JoinRoomResponse>>
    {
        private readonly IValidatorService<JoinRoomCommand> _validator;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public Handler(IValidatorService<JoinRoomCommand> validator, IUserRepository userRepository, IRoomRepository roomRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<Response<JoinRoomResponse>> Handle(JoinRoomCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return new NotFound();

            if (user.Rooms.Contains(request.RoomId))
                return new JoinRoomResponse { User = user, Room = null };

            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
                return new NotFound();

            user.Rooms.Add(request.RoomId);
            room.Users.Add(request.UserId);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _roomRepository.UpdateAsync(room, cancellationToken);

            return new JoinRoomResponse { User = user, Room = room };
        }
    }
}

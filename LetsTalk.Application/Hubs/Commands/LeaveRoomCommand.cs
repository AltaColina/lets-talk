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

public sealed partial class LeaveRoomResponse
{
    [MemberNotNullWhen(true, nameof(Room))]
    public bool HasUserLeft { get => Room is not null; }
    public required User User { get; init; }
    public required Room? Room { get; init; }
}

public sealed class LeaveRoomCommand : IRequest<Response<LeaveRoomResponse>>
{
    public required string RoomId { get; init; }
    public required string UserId { get; init; }

    public sealed class Validator : AbstractValidator<LeaveRoomCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.RoomId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<LeaveRoomCommand, Response<LeaveRoomResponse>>
    {
        private readonly IValidatorService<LeaveRoomCommand> _validator;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public Handler(IValidatorService<LeaveRoomCommand> validator, IUserRepository userRepository, IRoomRepository roomRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<Response<LeaveRoomResponse>> Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return new NotFound();

            if (!user.Rooms.Contains(request.RoomId))
                return new LeaveRoomResponse { User = user, Room = null };

            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
                return new NotFound();

            user.Rooms.Remove(request.RoomId);
            room.Users.Remove(request.UserId);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _roomRepository.UpdateAsync(room, cancellationToken);

            return new LeaveRoomResponse { User = user, Room = room };
        }
    }
}

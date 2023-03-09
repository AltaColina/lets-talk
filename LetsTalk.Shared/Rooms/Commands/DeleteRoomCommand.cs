using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Rooms.Commands;

public sealed class DeleteRoomCommand : IRequest
{
    public string Id { get; init; } = null!;

    public sealed class Validator : AbstractValidator<DeleteRoomCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteRoomCommand>
    {
        private readonly IRoomRepository _roomRepository;

        public Handler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken);
            if (room is null)
                throw new NotFoundException($"Room {request.Id} does not exist");
            await _roomRepository.DeleteAsync(room, cancellationToken);
        }
    }
}
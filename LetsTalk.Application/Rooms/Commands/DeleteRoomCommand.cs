using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Rooms.Commands;

public sealed class DeleteRoomCommand : IRequest<Response<Success>>
{
    public required string Id { get; init; }

    public sealed class Validator : AbstractValidator<DeleteRoomCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteRoomCommand, Response<Success>>
    {
        private readonly IValidatorService<DeleteRoomCommand> _validator;
        private readonly IRoomRepository _roomRepository;

        public Handler(IValidatorService<DeleteRoomCommand> validator, IRoomRepository roomRepository)
        {
            _validator = validator;
            _roomRepository = roomRepository;
        }

        public async Task<Response<Success>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken);
            if (room is null)
                return new NotFound();

            await _roomRepository.DeleteAsync(room, cancellationToken);

            return new Success();
        }
    }
}
using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;

namespace LetsTalk.Rooms.Commands;

public sealed class CreateRoomCommand : IRequest<Response<RoomDto>>, IMapTo<Room>
{
    public required string Name { get; init; }

    public sealed class Validator : AbstractValidator<CreateRoomCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Name).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<CreateRoomCommand, Response<RoomDto>>
    {
        private readonly IValidatorService<CreateRoomCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IValidatorService<CreateRoomCommand> validator, IMapper mapper, IRoomRepository roomRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<Response<RoomDto>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            if (await _roomRepository.GetByNameAsync(request.Name, cancellationToken) is not null)
                return new AlreadyExists();

            var room = _mapper.Map<Room>(request);
            room.Name ??= room.Id;
            await _roomRepository.AddAsync(room, cancellationToken);
            return _mapper.Map<RoomDto>(room);
        }
    }
}

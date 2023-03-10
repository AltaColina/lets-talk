using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Rooms.Commands;

public sealed class CreateRoomCommand : IRequest<RoomDto>, IMapTo<Room>
{
    public required string Name { get; init; }

    public sealed class Validator : AbstractValidator<CreateRoomCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Name).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<CreateRoomCommand, RoomDto>
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<RoomDto> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            if (await _roomRepository.GetByNameAsync(request.Name, cancellationToken) is not null)
                throw ExceptionFor<Room>.AlreadyExists(r => r.Name, request.Name);
            var room = _mapper.Map<Room>(request);
            room.Name ??= room.Id;
            await _roomRepository.AddAsync(room, cancellationToken);
            return _mapper.Map<RoomDto>(room);
        }
    }
}

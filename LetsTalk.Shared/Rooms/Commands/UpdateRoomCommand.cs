using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Rooms.Commands;

public sealed class UpdateRoomCommand : IRequest<RoomDto>, IMapTo<Room>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;

    public sealed class Validator : AbstractValidator<UpdateRoomCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Name).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<UpdateRoomCommand, RoomDto>
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<RoomDto> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken);
            if (room is null)
                throw new NotFoundException($"Room {request.Id} does not exist");
            room = _mapper.Map(request, room);
            await _roomRepository.UpdateAsync(room, cancellationToken);
            return _mapper.Map<RoomDto>(room);
        }
    }
}
using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using MediatR;
using System.Text.Json.Serialization;

namespace LetsTalk.Rooms.Commands;

public sealed class UpdateRoomCommand : IRequest<RoomDto>, IMapTo<Room>
{
    [JsonIgnore]
    public required string Id { get; set; }
    public required string Name { get; init; }

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
            var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw ExceptionFor<Room>.NotFound(r => r.Id, request.Id);
            room = _mapper.Map(request, room);
            await _roomRepository.UpdateAsync(room, cancellationToken);
            return _mapper.Map<RoomDto>(room);
        }
    }
}
using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;
using System.Text.Json.Serialization;

namespace LetsTalk.Rooms.Commands;

public sealed class UpdateRoomCommand : IRequest<Response<RoomDto>>, IMapTo<Room>
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

    public sealed class Handler : IRequestHandler<UpdateRoomCommand, Response<RoomDto>>
    {
        private readonly IValidatorService<UpdateRoomCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IValidatorService<UpdateRoomCommand> validator, IMapper mapper, IRoomRepository roomRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<Response<RoomDto>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken);
            if (room is null)
                return new NotFound();

            room = _mapper.Map(request, room);
            await _roomRepository.UpdateAsync(room, cancellationToken);
            return _mapper.Map<RoomDto>(room);
        }
    }
}
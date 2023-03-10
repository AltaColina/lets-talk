using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Rooms.Queries;

public sealed class GetRoomByIdQuery : IRequest<RoomDto>
{
    public required string RoomId { get; init; }

    public sealed class Validator : AbstractValidator<GetRoomByIdQuery>
    {
        public Validator()
        {
            RuleFor(e => e.RoomId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetRoomByIdQuery, RoomDto>
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<RoomDto> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
                throw ExceptionFor<Room>.NotFound(r => r.Id, request.RoomId);
            return _mapper.Map<RoomDto>(room);
        }
    }
}
using AutoMapper;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Rooms.Queries;

public sealed class GetRoomsResponse
{
    public required List<RoomDto> Rooms { get; init; }
}

public sealed class GetRoomQuery : IRequest<GetRoomsResponse>
{
    public sealed class Handler : IRequestHandler<GetRoomQuery, GetRoomsResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<GetRoomsResponse> Handle(GetRoomQuery request, CancellationToken cancellationToken)
        {
            return new GetRoomsResponse { Rooms = _mapper.Map<List<RoomDto>>(await _roomRepository.ListAsync(cancellationToken)) };
        }
    }
}

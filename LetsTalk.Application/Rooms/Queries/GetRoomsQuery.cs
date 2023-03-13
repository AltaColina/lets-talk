using AutoMapper;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Rooms.Queries;

public sealed class GetRoomsResponse
{
    public required List<RoomDto> Rooms { get; init; }
}

public sealed class GetRoomsQuery : IRequest<GetRoomsResponse>
{
    public sealed class Handler : IRequestHandler<GetRoomsQuery, GetRoomsResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<GetRoomsResponse> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            var rooms = await _roomRepository.ListAsync(cancellationToken);

            return new GetRoomsResponse { Rooms = _mapper.Map<List<RoomDto>>(rooms) };
        }
    }
}

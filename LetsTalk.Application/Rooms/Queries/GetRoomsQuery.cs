using AutoMapper;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;

namespace LetsTalk.Rooms.Queries;

public sealed class GetRoomsResponse
{
    public required List<RoomDto> Rooms { get; init; }
}

public sealed class GetRoomsQuery : IRequest<Response<GetRoomsResponse>>
{
    public sealed class Handler : IRequestHandler<GetRoomsQuery, Response<GetRoomsResponse>>
    {
        private readonly IValidatorService<GetRoomsQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IValidatorService<GetRoomsQuery> validator, IMapper mapper, IRoomRepository roomRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<Response<GetRoomsResponse>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var rooms = await _roomRepository.ListAsync(cancellationToken);

            return new GetRoomsResponse { Rooms = _mapper.Map<List<RoomDto>>(rooms) };
        }
    }
}

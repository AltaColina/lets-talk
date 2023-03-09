using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Rooms;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Users.Queries;

public sealed class GetUserRoomsResponse
{
    public List<RoomDto> Rooms { get; init; } = null!;
}

public sealed class GetUserRoomsQuery : IRequest<GetUserRoomsResponse>
{
    public string UserId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetUserRoomsQuery>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
        }
    }

    public sealed class Specification : Specification<Room>
    {
        public Specification(ICollection<string> roomIds)
        {
            Query.Where(room => roomIds.Contains(room.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetUserRoomsQuery, GetUserRoomsResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IUserRepository userRepository, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<GetUserRoomsResponse> Handle(GetUserRoomsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new NotFoundException($"User {request.UserId} does not exist");

            var rooms = await _roomRepository.ListAsync(new Specification(user.Rooms), cancellationToken);
            return new GetUserRoomsResponse { Rooms = _mapper.Map<List<RoomDto>>(rooms) };
        }
    }
}

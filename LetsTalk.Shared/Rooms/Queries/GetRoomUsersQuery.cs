using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Rooms.Queries;

public sealed class GetRoomUsersResponse
{
    public List<UserDto> Users { get; init; } = null!;
}

public sealed class GetRoomUsersQuery : IRequest<GetRoomUsersResponse>
{
    public string RoomId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetRoomUsersQuery>
    {
        public Validator()
        {
            RuleFor(e => e.RoomId).NotEmpty();
        }
    }

    public sealed class Specification : Specification<User>
    {
        public Specification(ICollection<string> userIds)
        {
            Query.Where(user => userIds.Contains(user.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetRoomUsersQuery, GetRoomUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;

        public Handler(IMapper mapper, IRoomRepository roomRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
        }

        public async Task<GetRoomUsersResponse> Handle(GetRoomUsersQuery request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
                throw new NotFoundException($"Room {request.RoomId} does not exist");

            var users = await _userRepository.ListAsync(new Specification(room.Users), cancellationToken);
            return new GetRoomUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

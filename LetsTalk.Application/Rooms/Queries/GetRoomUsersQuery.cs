using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Rooms.Queries;

public sealed class GetRoomUsersResponse
{
    public required List<UserDto> Users { get; init; }
}

public sealed class GetRoomUsersQuery : IRequest<Response<GetRoomUsersResponse>>
{
    public required string RoomId { get; init; }

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

    public sealed class Handler : IRequestHandler<GetRoomUsersQuery, Response<GetRoomUsersResponse>>
    {
        private readonly IValidatorService<GetRoomUsersQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;

        public Handler(IValidatorService<GetRoomUsersQuery> validator, IMapper mapper, IRoomRepository roomRepository, IUserRepository userRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
        }

        public async Task<Response<GetRoomUsersResponse>> Handle(GetRoomUsersQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
                return new NotFound();

            var users = await _userRepository.ListAsync(new Specification(room.Users), cancellationToken);
            return new GetRoomUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

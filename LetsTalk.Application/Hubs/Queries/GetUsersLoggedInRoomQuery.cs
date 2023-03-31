using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Hubs.Queries;

public sealed class GetUsersLoggedInRoomResponse
{
    public required List<UserDto> Users { get; init; }
}

public sealed class GetUsersLoggedInRoomQuery : IRequest<Response<GetUsersLoggedInRoomResponse>>
{
    public required string RoomId { get; init; }

    public sealed class Validator : AbstractValidator<GetUsersLoggedInRoomQuery>
    {
        public Validator()
        {
            RuleFor(e => e.RoomId).NotEmpty();
        }
    }

    public sealed class Specification : Specification<User>
    {
        public Specification(IReadOnlyCollection<string> userIds)
        {
            Query.Where(user => userIds.Contains(user.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetUsersLoggedInRoomQuery, Response<GetUsersLoggedInRoomResponse>>
    {
        private readonly IValidatorService<GetUsersLoggedInRoomQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public Handler(
            IValidatorService<GetUsersLoggedInRoomQuery> validator,
            IMapper mapper,
            IHubConnectionManager connectionManager,
            IUserRepository userRepository,
            IRoomRepository roomRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<Response<GetUsersLoggedInRoomResponse>> Handle(GetUsersLoggedInRoomQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
                return new NotFound();

            var userIds = _connectionManager.GetUserIds().Intersect(room.Users).ToList();
            var users = await _userRepository.ListAsync(new Specification(userIds), cancellationToken);
            return new GetUsersLoggedInRoomResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

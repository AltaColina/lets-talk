using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Rooms.Queries;

public sealed class GetRoomsWithoutUserResponse
{
    public required List<RoomDto> Rooms { get; init; }
}

public sealed class GetRoomsWithoutUserQuery : IRequest<Response<GetRoomsWithoutUserResponse>>
{
    public required string UserId { get; init; }

    public sealed class Validator : AbstractValidator<GetRoomsWithoutUserQuery>
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
            Query.Where(room => !roomIds.Contains(room.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetRoomsWithoutUserQuery, Response<GetRoomsWithoutUserResponse>>
    {
        private readonly IValidatorService<GetRoomsWithoutUserQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public Handler(IValidatorService<GetRoomsWithoutUserQuery> validator, IMapper mapper, IUserRepository userRepository, IRoomRepository roomRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<Response<GetRoomsWithoutUserResponse>> Handle(GetRoomsWithoutUserQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return new NotFound();

            var rooms = await _roomRepository.ListAsync(new Specification(user.Rooms), cancellationToken);
            return new GetRoomsWithoutUserResponse { Rooms = _mapper.Map<List<RoomDto>>(rooms) };
        }
    }
}

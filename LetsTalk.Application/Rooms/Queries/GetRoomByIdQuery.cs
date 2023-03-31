using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Rooms.Queries;

public sealed class GetRoomByIdQuery : IRequest<Response<RoomDto>>
{
    public required string RoomId { get; init; }

    public sealed class Validator : AbstractValidator<GetRoomByIdQuery>
    {
        public Validator()
        {
            RuleFor(e => e.RoomId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetRoomByIdQuery, Response<RoomDto>>
    {
        private readonly IValidatorService<GetRoomByIdQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IValidatorService<GetRoomByIdQuery> validator, IMapper mapper, IRoomRepository roomRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<Response<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
                return new NotFound();

            return _mapper.Map<RoomDto>(room);
        }
    }
}
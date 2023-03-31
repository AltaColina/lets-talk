using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Users.Queries;

public sealed class GetUserByIdQuery : IRequest<Response<UserDto>>
{
    public required string UserId { get; init; }

    public sealed class Validator : AbstractValidator<GetUserByIdQuery>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
        }

        public static Validator Instance { get; } = new();
    }

    public sealed class Handler : IRequestHandler<GetUserByIdQuery, Response<UserDto>>
    {
        private readonly IValidatorService<GetUserByIdQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public Handler(IValidatorService<GetUserByIdQuery> validator, IMapper mapper, IUserRepository userRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Response<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return new NotFound();

            return _mapper.Map<UserDto>(user);
        }
    }
}

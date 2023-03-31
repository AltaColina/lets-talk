using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;

namespace LetsTalk.Users.Commands;

public sealed class CreateUserCommand : IRequest<Response<UserDto>>, IMapTo<User>
{
    public required string UserName { get; init; }

    public required string Email { get; init; }

    [SensitiveData]
    public required string Password { get; init; }

    public sealed partial class Validator : AbstractValidator<CreateUserCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserName).NotEmpty().Matches(RegexExpr.UserName());
            RuleFor(e => e.Email).NotEmpty().Matches(RegexExpr.Email());
            //RuleFor(e => e.Password).NotEmpty().Matches(RegexExpr.Password());
        }

        public static Validator Instance { get; } = new();
    }

    public sealed class Handler : IRequestHandler<CreateUserCommand, Response<UserDto>>
    {
        private readonly IValidatorService<CreateUserCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHandler _passwordHandler;

        public Handler(IValidatorService<CreateUserCommand> validator, IMapper mapper, IUserRepository userRepository, IPasswordHandler passwordHandler)
        {
            _validator = validator;
            _mapper = mapper;
            _userRepository = userRepository;
            _passwordHandler = passwordHandler;
        }

        public async Task<Response<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            if (await _userRepository.GetByNameAsync(request.UserName, cancellationToken) is not null)
                return new AlreadyExists();

            var user = await _userRepository.AddAsync(new User
            {
                Name = request.UserName,
                Email = request.Email,
                Secret = _passwordHandler.Encrypt(request.Password),
                Roles = { "user" }
            }, cancellationToken);

            return _mapper.Map<UserDto>(user);
        }
    }
}

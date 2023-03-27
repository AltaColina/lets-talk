using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Security;
using LetsTalk.Services;
using MediatR;

namespace LetsTalk.Users.Commands;
public sealed class CreateUserCommand : IRequest<UserDto>, IMapTo<User>
{
    public required string UserName { get; init; }

    public required string Email { get; init; }

    [SensitiveData]
    public required string Password { get; init; }

    public sealed partial class Validator : AbstractValidator<CreateUserCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserName).NotEmpty().Must(e => e is not null && RegexValidation.UserName.IsMatch(e));
            RuleFor(e => e.Email).EmailAddress();
            RuleFor(e => e.Password).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHandler _passwordHandler;

        public Handler(IMapper mapper, IUserRepository userRepository, IPasswordHandler passwordHandler)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _passwordHandler = passwordHandler;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if ((await _userRepository.GetByNameAsync(request.UserName, cancellationToken)) is not null)
                throw ExceptionFor<User>.AlreadyExists(u => u.Name, request.UserName);

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

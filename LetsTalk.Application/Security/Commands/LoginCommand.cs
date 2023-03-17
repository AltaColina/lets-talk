using FluentValidation;
using LetsTalk.Services;
using MediatR;

namespace LetsTalk.Security.Commands;

public sealed class LoginCommand : IRequest<Authentication>
{
    [SensitiveData(ShowFirst = 1, ShowLast = 1)]
    public required string Username { get; init; }

    [SensitiveData]
    public required string Password { get; init; }

    public sealed class Validator : AbstractValidator<LoginCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Username).NotEmpty();
            RuleFor(e => e.Password).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<LoginCommand, Authentication>
    {
        private readonly IAuthenticationManager _authenticationManager;

        public Handler(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        public async Task<Authentication> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationManager.AuthenticateAsync(request);
        }
    }
}

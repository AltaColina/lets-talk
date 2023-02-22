using FluentValidation;
using LetsTalk.Interfaces;
using MediatR;

namespace LetsTalk.Security.Commands;

public sealed class LoginCommand : IRequest<Authentication>
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;

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

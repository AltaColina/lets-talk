using MediatR;
using LetsTalk.Models;
using LetsTalk.Interfaces;
using FluentValidation;

namespace LetsTalk.Commands.Auths;

public sealed class LoginRequest : IRequest<Authentication>
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;

    public sealed class Validator : AbstractValidator<LoginRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Username).NotEmpty();
            RuleFor(e => e.Password).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<LoginRequest, Authentication>
    {
        private readonly IAuthenticationManager _authenticationManager;

        public Handler(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        public async Task<Authentication> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            return await _authenticationManager.AuthenticateAsync(request);
        }
    }
}

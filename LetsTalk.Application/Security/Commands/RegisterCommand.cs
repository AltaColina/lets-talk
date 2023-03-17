using FluentValidation;
using LetsTalk.Services;
using MediatR;
using System.Text.RegularExpressions;

namespace LetsTalk.Security.Commands;

public sealed partial class RegisterCommand : IRequest<Authentication>
{
    public required string Username { get; init; }

    [SensitiveData]
    public required string Password { get; init; }

    public sealed partial class Validator : AbstractValidator<RegisterCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Username).NotEmpty().Must(e => e is not null && UsernameRegex().IsMatch(e));
            RuleFor(e => e.Password).NotEmpty();
        }

        [GeneratedRegex("^[a-zA-Z][a-z0-9_-]{3,15}$")]
        private static partial Regex UsernameRegex();
    }

    public sealed class Handler : IRequestHandler<RegisterCommand, Authentication>
    {
        private readonly IAuthenticationManager _authenticationManager;

        public Handler(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        public async Task<Authentication> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationManager.AuthenticateAsync(request);
        }
    }
}

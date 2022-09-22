using FluentValidation;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;
using System.Text.RegularExpressions;

namespace LetsTalk.Commands.Auths;

public sealed class RegisterRequest : IRequest<Authentication>
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string? Name { get; init; }

    public sealed class Validator : AbstractValidator<RegisterRequest>
    {
        // This can be made compile time in .NET 7.
        private static readonly Regex UsernameRegex = new("^[a-zA-Z][a-z0-9_-]{3,15}$");

        public Validator()
        {
            RuleFor(e => e.Username).NotEmpty().Must(e => e is not null && UsernameRegex.IsMatch(e));
            RuleFor(e => e.Password).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<RegisterRequest, Authentication>
    {
        private readonly IAuthenticationManager _authenticationManager;

        public Handler(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        public async Task<Authentication> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            return await _authenticationManager.AuthenticateAsync(request);
        }
    }
}

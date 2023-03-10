using FluentValidation;
using LetsTalk.Services;
using MediatR;

namespace LetsTalk.Security.Commands;

public sealed class RefreshCommand : IRequest<Authentication>
{
    public required string Username { get; init; }
    public required string RefreshToken { get; init; }

    public sealed class Validator : AbstractValidator<RefreshCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Username).NotEmpty();
            RuleFor(e => e.RefreshToken).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<RefreshCommand, Authentication>
    {
        private readonly IAuthenticationManager _authenticationManager;

        public Handler(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        public async Task<Authentication> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationManager.AuthenticateAsync(request);
        }
    }
}

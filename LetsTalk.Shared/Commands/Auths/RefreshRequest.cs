﻿using FluentValidation;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands.Auths;

public sealed class RefreshRequest : IRequest<Authentication>
{
    public string Username { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;

    public sealed class Validator : AbstractValidator<RefreshRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Username).NotEmpty();
            RuleFor(e => e.RefreshToken).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<RefreshRequest, Authentication>
    {
        private readonly IAuthenticationManager _authenticationManager;

        public Handler(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        public async Task<Authentication> Handle(RefreshRequest request, CancellationToken cancellationToken)
        {
            return await _authenticationManager.AuthenticateAsync(request);
        }
    }
}

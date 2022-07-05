using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;
using System.Text.RegularExpressions;

namespace LetsTalk.Commands;

public sealed class RegisterRequestHandler : IRequestHandler<RegisterRequest, AuthenticationResponse>
{
    private readonly IAuthenticationManager _authenticationManager;

    public RegisterRequestHandler(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    public async Task<AuthenticationResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        return await _authenticationManager.AuthenticateAsync(request);
    }
}

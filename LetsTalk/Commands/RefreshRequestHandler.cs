using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class RefreshRequestHandler : IRequestHandler<RefreshRequest, RefreshResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationManager _authenticationManager;

    public RefreshRequestHandler(IUserRepository userRepository, IAuthenticationManager authenticationManager)
    {
        _userRepository = userRepository;
        _authenticationManager = authenticationManager;
    }

    public async Task<RefreshResponse> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        var token = await _authenticationManager.RefreshAsync(request.Username, request.RefreshToken);
        if (token is null)
            throw new ForbiddenException("Incorrect username or password");

        // Update user.
        var user = (await _userRepository.GetAsync(request.Username))!;
        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.Id == request.RefreshToken || token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(new RefreshToken
        {
            Id = token.RefreshToken,
            ExpiresIn = DateTime.UnixEpoch.AddSeconds(token.RefreshTokenExpiresIn)
        });
        await _userRepository.UpdateAsync(user);

        return new RefreshResponse
        {
            Person = new Person { Username = request.Username },
            Token = token
        };
    }
}
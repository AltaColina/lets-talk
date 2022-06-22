using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LetsTalk.Services;

public sealed class JwtAuthenticationManager : IAuthenticationManager
{
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly IPasswordHandler _passwordHandler;
    private readonly IUserRepository _userRepository;

    public JwtAuthenticationManager(SecurityKey securityKey, IPasswordHandler passwordHandler, IUserRepository userRepository)
    {
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        _tokenHandler = new JwtSecurityTokenHandler();
        _passwordHandler = passwordHandler;
        _userRepository = userRepository;
    }

    public string? Authenticate(string username, string password)
    {
        var user = _userRepository.Get(username);
        if (user is null || !_passwordHandler.IsValid(user.Password, password, username))
            return null;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = _signingCredentials,
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }
}

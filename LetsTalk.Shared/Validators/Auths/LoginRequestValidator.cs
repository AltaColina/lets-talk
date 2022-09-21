using FluentValidation;
using LetsTalk.Dtos.Auths;

namespace LetsTalk.Validators.Auths;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(e => e.Username).NotEmpty();
        RuleFor(e => e.Password).NotEmpty();
    }
}
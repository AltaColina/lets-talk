using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(e => e.Username).NotEmpty();
        RuleFor(e => e.Password).NotEmpty();
    }
}
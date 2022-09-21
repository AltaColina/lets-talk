using FluentValidation;
using LetsTalk.Dtos.Auths;

namespace LetsTalk.Validators.Auths;

public sealed class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(e => e.Username).NotEmpty();
        RuleFor(e => e.RefreshToken).NotEmpty();
    }
}
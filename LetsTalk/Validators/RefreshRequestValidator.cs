using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(e => e.Username).NotEmpty();
        RuleFor(e => e.RefreshToken).NotEmpty();
    }
}
using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class RoleDeleteRequestValidator : AbstractValidator<RoleDeleteRequest>
{
    public RoleDeleteRequestValidator()
    {
        RuleFor(e => e.RoleId).NotEmpty();
    }
}
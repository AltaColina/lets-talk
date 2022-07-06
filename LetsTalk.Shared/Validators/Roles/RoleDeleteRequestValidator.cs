using FluentValidation;
using LetsTalk.Models.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class RoleDeleteRequestValidator : AbstractValidator<RoleDeleteRequest>
{
    public RoleDeleteRequestValidator()
    {
        RuleFor(e => e.RoleId).NotEmpty();
    }
}
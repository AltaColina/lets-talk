using FluentValidation;
using LetsTalk.Models.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class RoleUserGetRequestValidator : AbstractValidator<RoleUserGetRequest>
{
    public RoleUserGetRequestValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
    }
}

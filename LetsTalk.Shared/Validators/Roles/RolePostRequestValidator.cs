using FluentValidation;
using LetsTalk.Models.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class RolePostRequestValidator : AbstractValidator<RolePostRequest>
{
    public RolePostRequestValidator()
    {
        RuleFor(e => e.Role).NotNull().ChildRules(validator =>
        {
            validator.RuleFor(e => e.Id).NotEmpty();
            validator.RuleFor(e => e.Permissions).NotNull();
        });
    }
}
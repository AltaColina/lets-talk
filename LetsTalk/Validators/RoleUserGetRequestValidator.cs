using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class RoleUserGetRequestValidator : AbstractValidator<RoleUserGetRequest>
{
    public RoleUserGetRequestValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
    }
}

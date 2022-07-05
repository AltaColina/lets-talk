using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class RoleUserPutRequestValidator : AbstractValidator<RoleUserPutRequest>
{
    public RoleUserPutRequestValidator()
    {
        RuleFor(e => e.RoleId).NotEmpty();
        RuleFor(e => e.User).NotNull().ChildRules(validator =>
        {
            validator.RuleFor(e => e.Id).NotEmpty();
        });
    }
}
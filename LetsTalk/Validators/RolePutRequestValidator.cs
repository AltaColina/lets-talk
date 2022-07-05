using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class RolePutRequestValidator : AbstractValidator<RolePutRequest>
{
    public RolePutRequestValidator()
    {
        RuleFor(e => e.Role).NotNull().ChildRules(validator =>
        {
            validator.RuleFor(e => e.Id).NotEmpty();
            validator.RuleFor(e => e.Permissions).NotNull();
        });
    }
}
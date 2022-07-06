using FluentValidation;
using LetsTalk.Models.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class RoleUserPutRequestValidator : AbstractValidator<RoleUserPutRequest>
{
    public RoleUserPutRequestValidator()
    {
        RuleFor(e => e.RoleId).NotEmpty();
        RuleFor(e => e.UserId).NotEmpty();
    }
}
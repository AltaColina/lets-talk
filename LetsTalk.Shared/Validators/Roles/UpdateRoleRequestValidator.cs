using FluentValidation;
using LetsTalk.Dtos.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
        RuleFor(e => e.Name).NotEmpty();
        RuleFor(e => e.Permissions).NotNull();
    }
}
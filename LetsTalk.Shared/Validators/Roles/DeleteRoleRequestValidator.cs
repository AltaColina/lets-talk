using FluentValidation;
using LetsTalk.Dtos.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class DeleteRoleRequestValidator : AbstractValidator<DeleteRoleRequest>
{
    public DeleteRoleRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}
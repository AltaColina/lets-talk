using FluentValidation;
using LetsTalk.Dtos.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
        RuleFor(e => e.Name).NotEmpty();
        RuleFor(e => e.Permissions).NotNull();
    }
}
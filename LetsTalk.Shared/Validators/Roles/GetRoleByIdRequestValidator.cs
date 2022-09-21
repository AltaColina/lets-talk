using FluentValidation;
using LetsTalk.Dtos.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class GetRoleByIdRequestValidator : AbstractValidator<GetRoleByIdRequest>
{
    public GetRoleByIdRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}

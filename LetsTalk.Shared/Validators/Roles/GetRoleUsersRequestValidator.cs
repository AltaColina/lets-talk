using FluentValidation;
using LetsTalk.Dtos.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class GetRoleUsersRequestValidator : AbstractValidator<GetRoleUsersRequest>
{
    public GetRoleUsersRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}

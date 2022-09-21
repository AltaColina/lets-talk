using FluentValidation;
using LetsTalk.Dtos.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class GetRolesRequestValidator : AbstractValidator<GetRolesRequest>
{
    public GetRolesRequestValidator()
    {
    }
}

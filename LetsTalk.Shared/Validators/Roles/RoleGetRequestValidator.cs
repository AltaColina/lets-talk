using FluentValidation;
using LetsTalk.Models.Roles;

namespace LetsTalk.Validators.Roles;

public sealed class RoleGetRequestValidator : AbstractValidator<RoleGetRequest>
{
    public RoleGetRequestValidator()
    {
    }
}

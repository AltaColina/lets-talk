using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class UserRoleGetRequestValidator : AbstractValidator<UserRoleGetRequest>
{
    public UserRoleGetRequestValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
    }
}

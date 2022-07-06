using FluentValidation;
using LetsTalk.Models.Users;

namespace LetsTalk.Validators.Users;

public sealed class UserRoleGetRequestValidator : AbstractValidator<UserRoleGetRequest>
{
    public UserRoleGetRequestValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
    }
}
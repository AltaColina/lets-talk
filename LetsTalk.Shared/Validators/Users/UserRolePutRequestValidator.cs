using FluentValidation;
using LetsTalk.Models.Users;

namespace LetsTalk.Validators.Users;

public sealed class UserRolePutRequestValidator : AbstractValidator<UserRolePutRequest>
{
    public UserRolePutRequestValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.Roles).NotNull();
    }
}

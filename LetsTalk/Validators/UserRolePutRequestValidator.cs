using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class UserRolePutRequestValidator : AbstractValidator<UserRolePutRequest>
{
    public UserRolePutRequestValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.Roles).NotNull();
    }
}
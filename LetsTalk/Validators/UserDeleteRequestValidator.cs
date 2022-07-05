using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class UserDeleteRequestValidator : AbstractValidator<UserDeleteRequest>
{
    public UserDeleteRequestValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
    }
}

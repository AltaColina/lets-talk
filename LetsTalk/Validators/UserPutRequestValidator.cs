using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class UserPutRequestValidator : AbstractValidator<UserPutRequest>
{
    public UserPutRequestValidator()
    {
        RuleFor(e => e.User).NotNull().ChildRules(validator =>
        {
            validator.RuleFor(e => e.Id).NotEmpty();
            validator.RuleFor(e => e.Secret).NotEmpty();
            validator.RuleFor(e => e.CreationTime).NotEmpty();
            validator.RuleFor(e => e.LastLoginTime).NotEmpty();
            validator.RuleFor(e => e.Roles).NotNull();
            validator.RuleFor(e => e.RefreshTokens).NotNull();
        });
    }
}
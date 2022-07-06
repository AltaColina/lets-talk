using FluentValidation;
using LetsTalk.Models.Users;

namespace LetsTalk.Validators.Users;

public sealed class UserChatGetRequestValidator : AbstractValidator<UserChatGetRequest>
{
    public UserChatGetRequestValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
    }
}

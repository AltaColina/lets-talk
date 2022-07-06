using FluentValidation;
using LetsTalk.Models.Users;

namespace LetsTalk.Validators.Users;

public sealed class UserChatPutRequestValidator : AbstractValidator<UserChatPutRequest>
{
    public UserChatPutRequestValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.Chats).NotNull();
    }
}
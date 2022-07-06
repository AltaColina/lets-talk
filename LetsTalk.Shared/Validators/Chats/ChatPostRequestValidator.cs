using FluentValidation;
using LetsTalk.Models.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class ChatPostRequestValidator : AbstractValidator<ChatPostRequest>
{
    public ChatPostRequestValidator()
    {
        RuleFor(e => e.Chat).NotNull().ChildRules(validator =>
        {
            validator.RuleFor(e => e.Id).NotEmpty();
        });
    }
}

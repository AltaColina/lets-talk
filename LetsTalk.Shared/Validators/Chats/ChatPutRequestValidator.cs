using FluentValidation;
using LetsTalk.Models.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class ChatPutRequestValidator : AbstractValidator<ChatPutRequest>
{
    public ChatPutRequestValidator()
    {
        RuleFor(e => e.Chat).NotNull().ChildRules(validator =>
        {
            validator.RuleFor(e => e.Id).NotEmpty();
        });
    }
}
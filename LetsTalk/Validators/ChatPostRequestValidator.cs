using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

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

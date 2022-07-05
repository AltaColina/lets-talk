using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class ChatDeleteRequestValidator : AbstractValidator<ChatDeleteRequest>
{
    public ChatDeleteRequestValidator()
    {
        RuleFor(e => e.ChatId).NotEmpty();
    }
}
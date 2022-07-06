using FluentValidation;
using LetsTalk.Models.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class ChatDeleteRequestValidator : AbstractValidator<ChatDeleteRequest>
{
    public ChatDeleteRequestValidator()
    {
        RuleFor(e => e.ChatId).NotEmpty();
    }
}
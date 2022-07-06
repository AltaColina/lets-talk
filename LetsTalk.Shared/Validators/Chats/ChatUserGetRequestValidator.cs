using FluentValidation;
using LetsTalk.Models.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class ChatUserGetRequestValidator : AbstractValidator<ChatUserGetRequest>
{
    public ChatUserGetRequestValidator()
    {
        RuleFor(e => e.ChatId).NotEmpty();
    }
}

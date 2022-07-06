using FluentValidation;
using LetsTalk.Models.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class ChatUserPutRequestValidator : AbstractValidator<ChatUserPutRequest>
{
    public ChatUserPutRequestValidator()
    {
        RuleFor(e => e.ChatId).NotEmpty();
        RuleFor(e => e.UserId).NotEmpty();
    }
}
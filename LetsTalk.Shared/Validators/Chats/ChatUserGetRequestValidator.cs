using FluentValidation;
using LetsTalk.Dtos.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class ChatUserGetRequestValidator : AbstractValidator<GetChatUsersRequest>
{
    public ChatUserGetRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}

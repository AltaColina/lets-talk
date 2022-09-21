using FluentValidation;
using LetsTalk.Dtos.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class GetChatUsersRequestValidator : AbstractValidator<GetChatUsersRequest>
{
    public GetChatUsersRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}

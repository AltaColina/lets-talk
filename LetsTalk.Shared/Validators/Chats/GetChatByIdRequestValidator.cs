using FluentValidation;
using LetsTalk.Dtos.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class GetChatByIdRequestValidator : AbstractValidator<GetChatByIdRequest>
{
    public GetChatByIdRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}
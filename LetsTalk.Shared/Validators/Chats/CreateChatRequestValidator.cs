using FluentValidation;
using LetsTalk.Dtos.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class CreateChatRequestValidator : AbstractValidator<CreateChatRequest>
{
    public CreateChatRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}

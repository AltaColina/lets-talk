using FluentValidation;
using LetsTalk.Dtos.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class DeleteChatRequestValidator : AbstractValidator<DeleteChatRequest>
{
    public DeleteChatRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}
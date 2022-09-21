using FluentValidation;
using LetsTalk.Dtos.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class UpdateChatRequestValidator : AbstractValidator<UpdateChatRequest>
{
    public UpdateChatRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
        RuleFor(e => e.Name).NotEmpty();
    }
}
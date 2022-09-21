using FluentValidation;
using LetsTalk.Dtos.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class GetChatsRequestValidator : AbstractValidator<GetChatsRequest>
{
    public GetChatsRequestValidator()
    {
    }
}
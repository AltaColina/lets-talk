using FluentValidation;
using LetsTalk.Models.Chats;

namespace LetsTalk.Validators.Chats;

public sealed class ChatGetRequestValidator : AbstractValidator<ChatGetRequest>
{
    public ChatGetRequestValidator()
    {
    }
}
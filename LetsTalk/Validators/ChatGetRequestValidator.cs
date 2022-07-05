using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class ChatGetRequestValidator : AbstractValidator<ChatGetRequest>
{
    public ChatGetRequestValidator()
    {
    }
}

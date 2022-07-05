using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class UserGetRequestValidator : AbstractValidator<UserGetRequest>
{
    public UserGetRequestValidator()
    {
    }
}

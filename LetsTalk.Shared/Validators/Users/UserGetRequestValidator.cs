using FluentValidation;
using LetsTalk.Models.Users;

namespace LetsTalk.Validators.Users;

public sealed class UserGetRequestValidator : AbstractValidator<UserGetRequest>
{
    public UserGetRequestValidator()
    {
    }
}

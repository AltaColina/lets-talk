using FluentValidation;
using LetsTalk.Dtos.Users;

namespace LetsTalk.Validators.Users;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
        RuleFor(e => e.Roles).NotNull();
    }
}
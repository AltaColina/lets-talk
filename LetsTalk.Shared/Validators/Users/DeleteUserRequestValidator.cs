using FluentValidation;
using LetsTalk.Dtos.Users;

namespace LetsTalk.Validators.Users;

public sealed class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}

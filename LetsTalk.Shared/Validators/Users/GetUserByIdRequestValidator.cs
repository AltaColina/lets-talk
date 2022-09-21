using FluentValidation;
using LetsTalk.Dtos.Users;

namespace LetsTalk.Validators.Users;

public sealed class GetUserByIdRequestValidator : AbstractValidator<GetUserByIdRequest>
{
    public GetUserByIdRequestValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}

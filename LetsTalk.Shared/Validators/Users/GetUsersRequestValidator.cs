using FluentValidation;
using LetsTalk.Dtos.Users;

namespace LetsTalk.Validators.Users;

public sealed class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
{
    public GetUsersRequestValidator()
    {
    }
}

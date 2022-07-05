using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Validators;

public sealed class RoleGetRequestValidator : AbstractValidator<RoleGetRequest>
{
    public RoleGetRequestValidator()
    {
    }
}

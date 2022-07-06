using FluentValidation;
using LetsTalk.Models.Auths;
using System.Text.RegularExpressions;

namespace LetsTalk.Validators.Auths;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    // This can be made compile time in .NET 7.
    private static readonly Regex UsernameRegex = new("^[a-zA-Z][a-z0-9_-]{3,15}$");

    public RegisterRequestValidator()
    {
        RuleFor(e => e.Username).NotEmpty().Must(e => e is not null && UsernameRegex.IsMatch(e));
        RuleFor(e => e.Password).NotEmpty();
    }
}
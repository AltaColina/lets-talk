using LetsTalk.Users;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Services;
public interface IUserStore
{
    Task<User?> FindBySubjectIdAsync(string subjectId);
    Task<User?> FindByUsernameAsync(string username);
    Task<ValidateCredentialsResult> ValidateCredentialsAsync(string username, string password);
}

public readonly record struct ValidateCredentialsResult(User User)
{
    [MemberNotNullWhen(true, nameof(User))]
    public bool IsValid { get; init; }

    public User? User { get; init; } = User;

    public static ValidateCredentialsResult Invalid { get; } = new ValidateCredentialsResult();

    public static ValidateCredentialsResult Success(User user) => new(user) { IsValid = user is not null };
}
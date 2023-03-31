using LetsTalk.Users;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Services;
public interface IUserStore
{
    Task<User?> FindBySubjectIdAsync(string subjectId);
    Task<User?> FindByUsernameAsync(string username);
    Task<UserResult> ProvisionUserAsync(string username, string email, string password);
    Task<UserResult> ValidateCredentialsAsync(string username, string password);
}

public readonly record struct UserResult(User User)
{
    [MemberNotNullWhen(true, nameof(User))]
    public bool IsValid { get; init; }

    public User? User { get; init; } = User;

    public static UserResult Invalid { get; } = new UserResult();

    public static UserResult Success(User user) => new(user) { IsValid = user is not null };
}
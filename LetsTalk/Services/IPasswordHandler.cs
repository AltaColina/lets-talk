namespace LetsTalk.Services;

public interface IPasswordHandler
{
    string Encrypt(ReadOnlySpan<char> password, ReadOnlySpan<char> salt);
    bool IsValid(ReadOnlySpan<char> expectedPassword, ReadOnlySpan<char> password, ReadOnlySpan<char> salt);
}

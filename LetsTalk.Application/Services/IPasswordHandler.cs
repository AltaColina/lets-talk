namespace LetsTalk.Services;

public interface IPasswordHandler
{
    string Encrypt(ReadOnlySpan<char> password, ReadOnlySpan<char> salt);
    bool IsValid(ReadOnlySpan<char> secret, ReadOnlySpan<char> password, ReadOnlySpan<char> salt);
}

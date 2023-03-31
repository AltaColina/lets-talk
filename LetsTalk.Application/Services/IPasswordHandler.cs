namespace LetsTalk.Services;

public interface IPasswordHandler
{
    string Encrypt(ReadOnlySpan<char> password);
    bool IsValid(ReadOnlySpan<char> secret, ReadOnlySpan<char> password);
}
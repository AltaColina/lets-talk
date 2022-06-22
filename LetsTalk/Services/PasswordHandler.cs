using System.Security.Cryptography;
using System.Text;

namespace LetsTalk.Services;

public sealed class PasswordHandler : IPasswordHandler
{
    private readonly HashAlgorithm _hashAlgorithm;
    private readonly int _hashSize;
    private readonly int _passwordLength;

    public PasswordHandler(HashAlgorithm hashAlgorithm)
    {
        _hashAlgorithm = hashAlgorithm;
        _hashSize = _hashAlgorithm.HashSize / 8;
        _passwordLength = ((4 * (_hashAlgorithm.HashSize / 8) / 3) + 3) & ~3;
    }

    public string Encrypt(ReadOnlySpan<char> password, ReadOnlySpan<char> salt)
    {
        Span<char> encryptedPassword = stackalloc char[_passwordLength];
        EncryptPassword(password, salt, encryptedPassword);
        return new string(encryptedPassword);
    }

    private void EncryptPassword(ReadOnlySpan<char> password, ReadOnlySpan<char> salt, Span<char> encryptedPassword)
    {
        if (salt.Length == 0 || salt.Length > 64)
            throw new ArgumentException("Salt too small or too big");
        if (password.Length == 0 || password.Length > 64)
            throw new ArgumentException("Password too small or too big");

        Span<byte> passwordSpan = stackalloc byte[Encoding.UTF8.GetByteCount(password)];
        Span<byte> saltSpan = stackalloc byte[Encoding.UTF8.GetByteCount(salt)];
        Span<byte> encryptedSpan = stackalloc byte[_hashSize];
        EncryptPassword(passwordSpan, saltSpan, encryptedSpan);
        Convert.TryToBase64Chars(encryptedSpan, encryptedPassword, out _);
    }

    private void EncryptPassword(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, Span<byte> encryptedPassword)
    {
        Span<byte> buffer = stackalloc byte[password.Length + salt.Length];
        password.CopyTo(buffer);
        salt.CopyTo(buffer[password.Length..]);
        _hashAlgorithm.Initialize();
        _hashAlgorithm.TryComputeHash(buffer, encryptedPassword, out _);
    }

    public bool IsValid(ReadOnlySpan<char> expected, ReadOnlySpan<char> password, ReadOnlySpan<char> salt)
    {
        Span<char> encryptedPassword = stackalloc char[_passwordLength];
        EncryptPassword(password, salt, encryptedPassword);
        return expected.SequenceEqual(encryptedPassword);
    }
}

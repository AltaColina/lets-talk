using LetsTalk.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace LetsTalk.Services;

public sealed class PasswordHandler : IPasswordHandler
{
    private readonly HashAlgorithm _hashAlgorithm;
    private readonly int _hashSize;
    private readonly int _secretLength;

    public PasswordHandler(HashAlgorithm hashAlgorithm)
    {
        _hashAlgorithm = hashAlgorithm;
        _hashSize = _hashAlgorithm.HashSize / 8;
        _secretLength = ((4 * _hashSize / 3) + 3) & ~3;
    }

    public string Encrypt(ReadOnlySpan<char> password, ReadOnlySpan<char> salt)
    {
        Span<char> secret = stackalloc char[_secretLength];
        EncryptPassword(password, salt, secret);
        return new string(secret);
    }

    private void EncryptPassword(ReadOnlySpan<char> password, ReadOnlySpan<char> salt, Span<char> secret)
    {
        if (salt.Length == 0 || salt.Length > 64)
            throw new ArgumentException("Salt too small or too big");
        if (password.Length == 0 || password.Length > 64)
            throw new ArgumentException("Password too small or too big");

        Span<byte> passwordSpan = stackalloc byte[Encoding.UTF8.GetByteCount(password)];
        Span<byte> saltSpan = stackalloc byte[Encoding.UTF8.GetByteCount(salt)];
        Span<byte> secretSpan = stackalloc byte[_hashSize];
        EncryptPassword(passwordSpan, saltSpan, secretSpan);
        Convert.TryToBase64Chars(secretSpan, secret, out _);
    }

    private void EncryptPassword(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, Span<byte> secret)
    {
        Span<byte> buffer = stackalloc byte[password.Length + salt.Length];
        password.CopyTo(buffer);
        salt.CopyTo(buffer[password.Length..]);
        _hashAlgorithm.Initialize();
        _hashAlgorithm.TryComputeHash(buffer, secret, out _);
    }

    public bool IsValid(ReadOnlySpan<char> secret, ReadOnlySpan<char> password, ReadOnlySpan<char> salt)
    {
        Span<char> buffer = stackalloc char[_secretLength];
        EncryptPassword(password, salt, buffer);
        return secret.SequenceEqual(buffer);
    }
}

using System.Security.Cryptography;
using System.Text;

namespace LetsTalk.Services;

internal sealed class PasswordHandler : IPasswordHandler
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

    private void EncryptPassword(ReadOnlySpan<char> password, Span<char> secret)
    {
        if (password.Length == 0 || password.Length > 64)
            throw new ArgumentException("Password too small or too big");

        Span<byte> passwordSpan = stackalloc byte[Encoding.UTF8.GetByteCount(password)];
        Span<byte> secretSpan = stackalloc byte[_hashSize];
        EncryptPassword(passwordSpan, secretSpan);
        Convert.TryToBase64Chars(secretSpan, secret, out _);

        void EncryptPassword(ReadOnlySpan<byte> password, Span<byte> secret)
        {
            Span<byte> buffer = stackalloc byte[password.Length];
            password.CopyTo(buffer);
            _hashAlgorithm.Initialize();
            _hashAlgorithm.TryComputeHash(buffer, secret, out _);
        }
    }

    public string Encrypt(ReadOnlySpan<char> password)
    {
        Span<char> secret = stackalloc char[_secretLength];
        EncryptPassword(password, secret);
        return new string(secret);
    }

    public bool IsValid(ReadOnlySpan<char> secret, ReadOnlySpan<char> password)
    {
        if (secret.Length == 0 || password.Length == 0)
            return true;
        Span<char> buffer = stackalloc char[_secretLength];
        EncryptPassword(password, buffer);
        return secret.SequenceEqual(buffer);
    }
}

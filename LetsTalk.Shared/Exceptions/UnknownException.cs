namespace LetsTalk.Exceptions;

public sealed class UnknownException : Exception
{
    public UnknownException(string? message) : base(message)
    {

    }
}

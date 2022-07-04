using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class ChatPostResponse
{
    [NotNull] public Chat? Chat { get; init; }
}
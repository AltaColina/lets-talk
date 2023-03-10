using LetsTalk.Users;

namespace LetsTalk.Messaging;

public sealed class DisconnectMessage : Message
{
    public required UserTag User { get; init; }
}
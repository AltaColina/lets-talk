using LetsTalk.Users;

namespace LetsTalk.Messaging;

public sealed class ConnectMessage : Message
{
    public required UserTag User { get; init; }
}

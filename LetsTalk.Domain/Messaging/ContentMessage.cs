using LetsTalk.Users;

namespace LetsTalk.Messaging;
public sealed class ContentMessage : Message
{
    public required UserTag Sender { get; init; }

    public required string RoomId { get; init; }

    public required string ContentType { get; init; }

    public required byte[] Content { get; init; }
}

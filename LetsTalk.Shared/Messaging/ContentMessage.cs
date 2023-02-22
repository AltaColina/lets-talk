using LetsTalk.Users;

namespace LetsTalk.Messaging;
public sealed class ContentMessage : Message<byte[]>
{
    public UserDto Sender { get; init; } = null!;

    public string ChatId { get; init; } = null!;

    public string ContentType { get; init; } = null!;
}

using LetsTalk.Dtos;

namespace LetsTalk.Messaging.Abstract;

public abstract class UserMessage<T> : Message<T>
{
    public UserDto Sender { get; init; } = null!;
    public string ChatId { get; init; } = null!;
}

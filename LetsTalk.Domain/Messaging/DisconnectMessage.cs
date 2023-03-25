namespace LetsTalk.Messaging;

public sealed class DisconnectMessage : Message
{
    public required string UserId { get; init; }
    public required string UserName { get; init; }
    public string? UserImage { get; init; }
}
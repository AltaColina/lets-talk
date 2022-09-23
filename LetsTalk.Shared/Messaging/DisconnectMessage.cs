using LetsTalk.Dtos;
using LetsTalk.Messaging.Abstract;

namespace LetsTalk.Messaging;

public sealed class DisconnectMessage : Message<UserDto> { }
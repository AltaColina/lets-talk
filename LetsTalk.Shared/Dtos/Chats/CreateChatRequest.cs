﻿using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Dtos.Chats;

public sealed class CreateChatRequest : IRequest<ChatDto>, IMapTo<Chat>
{
    public string Id { get; set; } = null!;
    public string? Name { get; init; }
}

﻿using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class Chat
{
    [NotNull] public string? Id { get; init; }
}

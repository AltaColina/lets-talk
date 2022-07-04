﻿using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class Message
{
    [NotNull] public string? Username { get; init; }
    [NotNull] public string? Content { get; init; }
}
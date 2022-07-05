﻿using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class UserPutRequest : IRequest
{
    [NotNull] public User? User { get; init; }
}

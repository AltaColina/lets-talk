using LetsTalk.Models;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Interfaces;
public interface ILetsTalkSettings
{
    Authentication? Authentication { get; set; }

    [MemberNotNullWhen(true, nameof(Authentication))]
    bool IsAuthenticated { get => Authentication is not null; }

    Task<string?> ProvideToken();
}
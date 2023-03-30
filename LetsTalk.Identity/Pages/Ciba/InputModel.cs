// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace LetsTalk.Identity.Pages.Ciba;

public class InputModel
{
    public string? Button { get; set; }
    public IEnumerable<string> ScopesConsented { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Description { get; set; } = null!;
}
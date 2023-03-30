// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Identity.Pages.Register;

public class InputModel
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string ConfirmPassword { get; set; } = null!;

    public bool RememberLogin { get; set; }

    public string ReturnUrl { get; set; } = null!;

    public string? Button { get; set; }
}
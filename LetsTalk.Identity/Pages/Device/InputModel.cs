namespace LetsTalk.Identity.Pages.Device;

public class InputModel
{
    public string? Button { get; set; }
    public IEnumerable<string> ScopesConsented { get; set; } = null!;
    public bool RememberConsent { get; set; } = true;
    public string ReturnUrl { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string UserCode { get; set; } = null!;
}
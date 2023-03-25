using System.Text.RegularExpressions;

namespace LetsTalk.Security;
public static partial class RegexValidation
{
    public static Regex UserName { get; } = UsernameRegex();

    [GeneratedRegex("^[a-zA-Z][a-z0-9_-]{3,15}$")]
    private static partial Regex UsernameRegex();
}

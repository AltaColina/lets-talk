using System.Text.RegularExpressions;

namespace LetsTalk;
public static partial class RegexExpr
{
    [GeneratedRegex("^[a-zA-Z][a-z0-9_-]{3,15}$")]
    public static partial Regex UserName();

    [GeneratedRegex("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")]
    public static partial Regex Email();

    [GeneratedRegex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
    public static partial Regex Password();

    public static bool Matches(this string? input, Regex regex)
    {
        if (regex is null)
            throw new ArgumentNullException(nameof(regex));
        return input is not null && regex.IsMatch(input);
    }
}

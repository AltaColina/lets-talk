using IdentityModel;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace LetsTalk;
public static class PrincipalExtensions
{
    /// <summary>
    /// Gets roles.
    /// </summary>
    /// <param name="identity">The identity.</param>
    /// <returns></returns>
    [DebuggerStepThrough]
    public static IEnumerable<string> GetRoles(this IIdentity? identity)
    {
        var id = identity as ClaimsIdentity;
        return id is not null
            ? id.FindAll(JwtClaimTypes.Role).Select(c => c.Value)
            : Enumerable.Empty<string>();
    }
}

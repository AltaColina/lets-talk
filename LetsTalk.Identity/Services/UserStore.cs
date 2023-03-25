using LetsTalk.Repositories;
using LetsTalk.Users;

namespace LetsTalk.Services;

internal sealed class UserStore : IUserStore
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHandler _passwordHandler;

    public UserStore(IUserRepository userRepository, IPasswordHandler passwordHandler)
    {
        _userRepository = userRepository;
        _passwordHandler = passwordHandler;
    }

    public async Task<ValidateCredentialsResult> ValidateCredentialsAsync(string username, string password)
    {
        var user = await FindByUsernameAsync(username);

        if (user is null || !_passwordHandler.IsValid(user.Secret, password))
            return ValidateCredentialsResult.Invalid;

        return ValidateCredentialsResult.Success(user);
    }


    public Task<User?> FindBySubjectIdAsync(string subjectId)
    {
        return _userRepository.GetByIdAsync(subjectId);
    }

    public Task<User?> FindByUsernameAsync(string username)
    {
        return _userRepository.GetByNameAsync(username);
    }

    //public Task<User?> FindByExternalProviderAsync(string provider, string userId)
    //{
    //    return _userRepository.SingleOrDefaultAsync(x =>
    //        x.ProviderName == provider &&
    //        x.ProviderSubjectId == userId);
    //}

    //public async Task<User> AutoProvisionUserAsync(string provider, string userId, List<Claim> claims)
    //{
    //    // create a list of claims that we want to transfer into our store
    //    var filtered = new List<Claim>();

    //    foreach (var claim in claims)
    //    {
    //        // if the external system sends a display name - translate that to the standard OIDC name claim
    //        if (claim.Type == ClaimTypes.Name)
    //        {
    //            filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
    //        }
    //        // if the JWT handler has an outbound mapping to an OIDC claim use that
    //        else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.TryGetValue(claim.Type, out var value))
    //        {
    //            filtered.Add(new Claim(value, claim.Value));
    //        }
    //        // copy the claim as-is
    //        else
    //        {
    //            filtered.Add(claim);
    //        }
    //    }

    //    // if no display name was provided, try to construct by first and/or last name
    //    if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
    //    {
    //        var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
    //        var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
    //        if (first != null && last != null)
    //        {
    //            filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
    //        }
    //        else if (first != null)
    //        {
    //            filtered.Add(new Claim(JwtClaimTypes.Name, first));
    //        }
    //        else if (last != null)
    //        {
    //            filtered.Add(new Claim(JwtClaimTypes.Name, last));
    //        }
    //    }

    //    // create a new unique subject id
    //    var sub = Guid.NewGuid().ToString();

    //    // check if a display name is available, otherwise fallback to subject id
    //    var name = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? sub;

    //    // create new user
    //    var user = new User
    //    {
    //        Id = sub,
    //        Name = name,
    //        //ProviderName = provider,
    //        //ProviderSubjectId = userId,
    //        Claims = filtered
    //    };

    //    // add user to in-memory store
    //    await _userRepository.AddAsync(user);

    //    return user;
    //}
}
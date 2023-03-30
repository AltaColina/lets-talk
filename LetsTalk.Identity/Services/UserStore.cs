using LetsTalk.Repositories;
using LetsTalk.Users;

namespace LetsTalk.Services;

internal sealed class UserStore : IUserStore
{
    private readonly IUserStoreDefaults _userStoreDefaults;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHandler _passwordHandler;

    public UserStore(IUserStoreDefaults userStoreDefaults, IUserRepository userRepository, IPasswordHandler passwordHandler)
    {
        _userStoreDefaults = userStoreDefaults;
        _userRepository = userRepository;
        _passwordHandler = passwordHandler;
    }

    public async Task<UserResult> ProvisionUserAsync(string username, string email, string password)
    {
        if (String.IsNullOrWhiteSpace(username))
            return UserResult.Invalid;
        if (await _userRepository.GetByNameAsync(username) is not null)
            return UserResult.Invalid;
        var user = new User
        {
            Name = username,
            Email = email,
            Secret = _passwordHandler.Encrypt(password),
            IsActive = true,
            IsEmailVerified = true
        };
        user.Roles.UnionWith(await _userStoreDefaults.GetDefaultRolesAsync());
        user.Rooms.UnionWith(await _userStoreDefaults.GetDefaultRoomsAsync());
        return UserResult.Success(await _userRepository.AddAsync(user));
    }

    public async Task<UserResult> ValidateCredentialsAsync(string username, string password)
    {
        if (String.IsNullOrWhiteSpace(username))
            return UserResult.Invalid;

        var user = await FindByUsernameAsync(username);

        if (user is null || !_passwordHandler.IsValid(user.Secret, password))
            return UserResult.Invalid;

        return UserResult.Success(user);
    }


    public Task<User?> FindBySubjectIdAsync(string subjectId)
    {
        return _userRepository.GetByIdAsync(subjectId);
    }

    public Task<User?> FindByUsernameAsync(string username)
    {
        return _userRepository.GetByNameAsync(username);
    }
}
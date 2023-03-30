using Ardalis.Specification;
using LetsTalk.Repositories;

namespace LetsTalk.Services;

internal sealed class UserStoreDefaults : IUserStoreDefaults
{
    private readonly Configuration _configuration;
    private readonly IRoleRepository _roleRepository;
    private readonly IRoomRepository _roomRepository;
    private List<string>? _roles;
    private List<string>? _rooms;

    public UserStoreDefaults(IConfiguration configuration, IRoleRepository roleRepository, IRoomRepository roomRepository)
    {
        _configuration = configuration.GetRequiredSection("UserDefaults").Get<Configuration>() ?? throw new InvalidOperationException("Invalid user defaults");
        _roleRepository = roleRepository;
        _roomRepository = roomRepository;
    }

    public async Task<List<string>> GetDefaultRolesAsync()
    {
        return _roles ??= await _roleRepository.ListWherNameInAsync(_configuration.Roles);
    }

    public async Task<List<string>> GetDefaultRoomsAsync()
    {
        return _rooms ??= await _roomRepository.ListWherNameInAsync(_configuration.Rooms);
    }

    private sealed class Configuration
    {
        public required List<string> Roles { get; init; }
        public required List<string> Rooms { get; init; }
    }
}


file static class EntityRepositoryFilterExtensions
{
    public static async Task<List<string>> ListWherNameInAsync<T>(this IRepository<T> repository, IReadOnlyCollection<string> names) where T : Entity
    {
        var entities = await repository.ListAsync();
        return new List<string>(entities.Where(e => names.Contains(e.Name)).Select(e => e.Id));
    }
}
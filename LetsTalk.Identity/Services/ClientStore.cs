using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using LetsTalk.Repositories;

namespace LetsTalk.Services;

internal sealed class ClientStore : IClientStore
{
    private readonly IClientRepository _clientRepository;

    public ClientStore(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public Task<Client?> FindClientByIdAsync(string clientId)
    {
        return _clientRepository.GetByIdAsync(clientId);
    }
}

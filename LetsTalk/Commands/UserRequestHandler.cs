using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Models.Users;
using MediatR;

namespace LetsTalk.Commands;

public sealed class UserRequestHandler : IRequestHandler<UserGetRequest, UserGetResponse>,
                                         IRequestHandler<UserPostRequest>,
                                         IRequestHandler<UserPutRequest>,
                                         IRequestHandler<UserDeleteRequest>,
                                         IRequestHandler<UserRoleGetRequest, UserRoleGetResponse>,
                                         IRequestHandler<UserRolePutRequest>,
                                         IRequestHandler<UserChatGetRequest, UserChatGetResponse>,
                                         IRequestHandler<UserChatPutRequest>

{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Chat> _chatRepository;
    private readonly IRepository<Role> _roleRepository;

    public UserRequestHandler(IRepository<User> userRepository, IRepository<Chat> chatRepository, IRepository<Role> roleRepository)
    {
        _userRepository = userRepository;
        _chatRepository = chatRepository;
        _roleRepository = roleRepository;
    }

    public async Task<UserGetResponse> Handle(UserGetRequest request, CancellationToken cancellationToken)
    {
        if (String.IsNullOrWhiteSpace(request.UserId))
            return new UserGetResponse { Users = new List<User>(await _userRepository.GetAllAsync()) };

        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        return new UserGetResponse { Users = { user } };
    }

    public async Task<Unit> Handle(UserPostRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.User.Id);
        if (user is not null)
            throw new ConflictException($"User {request.User.Id} already exists");
        await _userRepository.InsertAsync(request.User);
        return Unit.Value;
    }

    public async Task<Unit> Handle(UserPutRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.User.Id);
        if (user is null)
            throw new NotFoundException($"User {request.User.Id} does not exist");
        await _userRepository.UpdateAsync(request.User);
        return Unit.Value;
    }

    public async Task<Unit> Handle(UserDeleteRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        await _userRepository.DeleteAsync(request.UserId);
        return Unit.Value;
    }

    public async Task<UserRoleGetResponse> Handle(UserRoleGetRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        var roles = (await _roleRepository.GetAllAsync()).Where(role => user.Roles.Contains(role.Id));
        return new UserRoleGetResponse { Roles = new(roles) };
    }

    public async Task<Unit> Handle(UserRolePutRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        // Get only valid roles.
        var roles = (await _roleRepository.GetAllAsync())
            .Select(role => role.Id)
            .Intersect(request.Roles);
        user.Roles.UnionWith(roles);
        await _userRepository.UpdateAsync(user);
        return Unit.Value;
    }

    public async Task<UserChatGetResponse> Handle(UserChatGetRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        var chats = (await _chatRepository.GetAllAsync()).Where(role => user.Roles.Contains(role.Id));
        return new UserChatGetResponse { Chats = new(chats) };
    }

    public async Task<Unit> Handle(UserChatPutRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        // Get only valid chats.
        var chats = (await _chatRepository.GetAllAsync())
            .Select(chat => chat.Id)
            .Intersect(request.Chats);
        user.Chats.UnionWith(chats);
        await _userRepository.UpdateAsync(user);
        return Unit.Value;
    }
}

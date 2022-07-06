using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models.Users;
using MediatR;

namespace LetsTalk.Commands.Users;

public sealed class UserChatPutRequestHandler : IRequestHandler<UserChatPutRequest>
{
    private readonly IUserRepository _userRepository;
    private readonly IChatRepository _chatRepository;

    public UserChatPutRequestHandler(IUserRepository userRepository, IChatRepository chatRepository)
    {
        _userRepository = userRepository;
        _chatRepository = chatRepository;
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

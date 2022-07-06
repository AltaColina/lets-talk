using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models.Users;
using MediatR;

namespace LetsTalk.Commands.Users;

public sealed class UserChatGetRequestHandler : IRequestHandler<UserChatGetRequest, UserChatGetResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IChatRepository _chatRepository;

    public UserChatGetRequestHandler(IUserRepository userRepository, IChatRepository chatRepository)
    {
        _userRepository = userRepository;
        _chatRepository = chatRepository;
    }

    public async Task<UserChatGetResponse> Handle(UserChatGetRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new NotFoundException($"User {request.UserId} does not exist");
        var chats = (await _chatRepository.GetAllAsync()).Where(role => user.Roles.Contains(role.Id));
        return new UserChatGetResponse { Chats = new(chats) };
    }
}

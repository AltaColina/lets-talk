using AutoMapper;
using LetsTalk.Dtos.Users;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands;

public sealed class UserRequestHandler : IRequestHandler<GetUsersRequest, GetUsersResponse>,
                                         IRequestHandler<GetUserByIdRequest, UserDto>,
                                         IRequestHandler<UpdateUserRequest>,
                                         IRequestHandler<DeleteUserRequest>

{
    private readonly IMapper _mapper;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Chat> _chatRepository;
    private readonly IRepository<Role> _roleRepository;

    public UserRequestHandler(IMapper mapper, IRepository<User> userRepository, IRepository<Chat> chatRepository, IRepository<Role> roleRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _chatRepository = chatRepository;
        _roleRepository = roleRepository;
    }

    public async Task<GetUsersResponse> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        return new GetUsersResponse { Users = _mapper.Map<List<UserDto>>(await _userRepository.ListAsync(cancellationToken)) };
    }

    public async Task<UserDto> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            throw new NotFoundException($"User {request.Id} does not exist");
        return _mapper.Map<UserDto>(user);
    }

    public async Task<Unit> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            throw new NotFoundException($"User {request.Id} does not exist");
        user = _mapper.Map(request, user);
        await _userRepository.UpdateAsync(user, cancellationToken);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            throw new NotFoundException($"User {request.Id} does not exist");
        await _userRepository.DeleteAsync(user, cancellationToken);
        return Unit.Value;
    }
}

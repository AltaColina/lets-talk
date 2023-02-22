using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Hubs.Commands;

public sealed class DisconnectCommand : IRequest<UserDto>
{
    public string UserId { get; init; } = null!;
    public string ConnectionId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<DisconnectCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.ConnectionId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DisconnectCommand, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IRepository<User> _userRepository;

        public Handler(IMapper mapper, IHubConnectionManager connectionManager, IRepository<User> userRepository)
        {
            _mapper = mapper;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(DisconnectCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new UnauthorizedException($"Invalid user '{request.UserId}'");

            _connectionManager.RemoveMapping(request.ConnectionId);

            return _mapper.Map<UserDto>(user);
        }
    }
}
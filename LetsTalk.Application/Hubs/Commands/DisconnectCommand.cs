using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Hubs.Commands;

public sealed class DisconnectCommandResponse
{
    public required UserDto User { get; init; }

    public required IReadOnlyCollection<string> Rooms { get; init; }
}

public sealed class DisconnectCommand : IRequest<DisconnectCommandResponse>
{
    public required string UserId { get; init; }
    public required string ConnectionId { get; init; }

    public sealed class Validator : AbstractValidator<DisconnectCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.ConnectionId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DisconnectCommand, DisconnectCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IUserRepository _userRepository;

        public Handler(IMapper mapper, IHubConnectionManager connectionManager, IUserRepository userRepository)
        {
            _mapper = mapper;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
        }

        public async Task<DisconnectCommandResponse> Handle(DisconnectCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken) ?? throw ExceptionFor<User>.Unauthorized();
            _connectionManager.RemoveMapping(request.ConnectionId);

            return new DisconnectCommandResponse
            {
                User = _mapper.Map<UserDto>(user),
                Rooms = user.Rooms
            };
        }
    }
}
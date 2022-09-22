using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands.Hubs;

public sealed class DisconnectRequest : IRequest<User>
{
    public string UserId { get; init; } = null!;
    public string ConnectionId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<DisconnectRequest>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.ConnectionId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DisconnectRequest, User>
    {
        private readonly IHubConnectionManager _connectionManager;
        private readonly IRepository<User> _userRepository;

        public Handler(IHubConnectionManager connectionManager, IRepository<User> userRepository)
        {
            _connectionManager = connectionManager;
            _userRepository = userRepository;
        }

        public async Task<User> Handle(DisconnectRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
                throw new UnauthorizedException($"Invalid user '{request.UserId}'");

            _connectionManager.RemoveMapping(request.ConnectionId);

            return user;
        }
    }
}
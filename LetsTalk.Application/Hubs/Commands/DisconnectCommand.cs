using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Hubs.Commands;

public sealed class DisconnectCommand : IRequest<Response<User>>
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

    public sealed class Handler : IRequestHandler<DisconnectCommand, Response<User>>
    {
        private readonly IValidatorService<DisconnectCommand> _validator;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IUserRepository _userRepository;

        public Handler(IValidatorService<DisconnectCommand> validator, IHubConnectionManager connectionManager, IUserRepository userRepository)
        {
            _validator = validator;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
        }

        public async Task<Response<User>> Handle(DisconnectCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return new NotFound();

            _connectionManager.RemoveMapping(request.ConnectionId);

            return user;
        }
    }
}
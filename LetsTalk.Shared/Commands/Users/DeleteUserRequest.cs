using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands.Users;

public sealed class DeleteUserRequest : IRequest
{
    public string Id { get; init; } = null!;

    public sealed class Validator : AbstractValidator<DeleteUserRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteUserRequest>
    {
        private readonly IRepository<User> _userRepository;

        public Handler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
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
}

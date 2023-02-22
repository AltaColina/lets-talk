using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using MediatR;

namespace LetsTalk.Users.Commands;

public sealed class DeleteUserCommand : IRequest
{
    public string Id { get; init; } = null!;

    public sealed class Validator : AbstractValidator<DeleteUserCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IRepository<User> _userRepository;

        public Handler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
                throw new NotFoundException($"User {request.Id} does not exist");
            await _userRepository.DeleteAsync(user, cancellationToken);
        }
    }
}

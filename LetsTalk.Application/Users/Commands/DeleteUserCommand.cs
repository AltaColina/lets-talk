using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Users.Commands;

public sealed class DeleteUserCommand : IRequest
{
    public required string Id { get; init; }

    public sealed class Validator : AbstractValidator<DeleteUserCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public Handler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
                throw ExceptionFor<User>.NotFound(r => r.Id, request.Id);
            await _userRepository.DeleteAsync(user, cancellationToken);
        }
    }
}

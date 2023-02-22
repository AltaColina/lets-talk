using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using MediatR;

namespace LetsTalk.Roles.Commands;

public sealed class DeleteRoleCommand : IRequest
{
    public string Id { get; init; } = null!;

    public sealed class Validator : AbstractValidator<DeleteRoleCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteRoleCommand>
    {
        private readonly IRepository<Role> _roleRepository;

        public Handler(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role is null)
                throw new NotFoundException($"Role {request.Id} does not exist");
            await _roleRepository.DeleteAsync(role, cancellationToken);
        }
    }
}
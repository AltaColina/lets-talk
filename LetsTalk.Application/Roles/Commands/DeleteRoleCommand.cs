using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Roles.Commands;

public sealed class DeleteRoleCommand : IRequest
{
    public required string Id { get; init; }

    public sealed class Validator : AbstractValidator<DeleteRoleCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public Handler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw ExceptionFor<Role>.NotFound(r => r.Id, request.Id);
            await _roleRepository.DeleteAsync(role, cancellationToken);
        }
    }
}
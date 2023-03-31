using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Roles.Commands;

public sealed class DeleteRoleCommand : IRequest<Response<Success>>
{
    public required string Id { get; init; }

    public sealed class Validator : AbstractValidator<DeleteRoleCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteRoleCommand, Response<Success>>
    {
        private readonly IValidatorService<DeleteRoleCommand> _validator;
        private readonly IRoleRepository _roleRepository;

        public Handler(IValidatorService<DeleteRoleCommand> validator, IRoleRepository roleRepository)
        {
            _validator = validator;
            _roleRepository = roleRepository;
        }

        public async Task<Response<Success>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role is null)
                return new NotFound();

            await _roleRepository.DeleteAsync(role, cancellationToken);

            return new Success();
        }
    }
}
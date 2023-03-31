using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;

namespace LetsTalk.Roles.Commands;

public sealed class CreateRoleCommand : IRequest<Response<RoleDto>>, IMapTo<Role>
{
    public required string Name { get; init; }

    public required List<string> Permissions { get; init; }

    public sealed class Validator : AbstractValidator<CreateRoleCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.Permissions).NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<CreateRoleCommand, Response<RoleDto>>
    {
        private readonly IValidatorService<CreateRoleCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public Handler(IValidatorService<CreateRoleCommand> validator, IMapper mapper, IRoleRepository roleRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<Response<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            if (await _roleRepository.GetByNameAsync(request.Name, cancellationToken) is not null)
                return new AlreadyExists();

            var role = _mapper.Map<Role>(request);
            role.Name ??= role.Id;
            role = await _roleRepository.AddAsync(role, cancellationToken);
            return _mapper.Map<RoleDto>(role);
        }
    }
}
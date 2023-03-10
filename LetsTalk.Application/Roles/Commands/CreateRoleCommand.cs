using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Roles.Commands;

public sealed class CreateRoleCommand : IRequest<RoleDto>, IMapTo<Role>
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

    public sealed class Handler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public Handler(IMapper mapper, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _roleRepository.GetByNameAsync(request.Name, cancellationToken) is not null)
                throw ExceptionFor<Role>.AlreadyExists(r => r.Name, request.Name);
            var role = _mapper.Map<Role>(request);
            role.Name ??= role.Id;
            role = await _roleRepository.AddAsync(role, cancellationToken);
            return _mapper.Map<RoleDto>(role);
        }
    }
}
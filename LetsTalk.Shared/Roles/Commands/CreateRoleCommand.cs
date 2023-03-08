using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Roles.Commands;

public sealed class CreateRoleCommand : IRequest<RoleDto>, IMapTo<Role>
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public List<string> Permissions { get; set; } = new();

    public sealed class Validator : AbstractValidator<CreateRoleCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
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
            if (await _roleRepository.GetByIdAsync(request.Id, cancellationToken) is not null)
                throw new ConflictException($"Role {request.Id} already exists");
            var role = _mapper.Map<Role>(request);
            role.Name ??= role.Id;
            role = await _roleRepository.AddAsync(role, cancellationToken);
            return _mapper.Map<RoleDto>(role);
        }
    }
}
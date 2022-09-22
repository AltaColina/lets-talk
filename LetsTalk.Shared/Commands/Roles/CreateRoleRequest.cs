using AutoMapper;
using FluentValidation;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands.Roles;

public sealed class CreateRoleRequest : IRequest<RoleDto>, IMapTo<Role>
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public List<string> Permissions { get; set; } = new();

    public sealed class Validator : AbstractValidator<CreateRoleRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.Permissions).NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<CreateRoleRequest, RoleDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Role> _roleRepository;

        public Handler(IMapper mapper, IRepository<Role> roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> Handle(CreateRoleRequest request, CancellationToken cancellationToken)
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
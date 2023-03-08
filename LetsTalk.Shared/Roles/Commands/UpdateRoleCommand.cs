using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Roles.Commands;

public sealed class UpdateRoleCommand : IRequest<RoleDto>, IMapTo<Role>
{
    public string Id { get; set; } = null!;
    public string Name { get; init; } = null!;
    public List<string> Permissions { get; set; } = new();

    public sealed class Validator : AbstractValidator<UpdateRoleCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.Permissions).NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<UpdateRoleCommand, RoleDto>
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public Handler(IMapper mapper, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role is null)
                throw new NotFoundException($"Role {request.Id} does not exist");
            role = _mapper.Map(request, role);
            await _roleRepository.UpdateAsync(role, cancellationToken);
            return _mapper.Map<RoleDto>(role);
        }
    }
}

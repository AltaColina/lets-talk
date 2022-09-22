using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands.Roles;

public sealed class UpdateRoleRequest : IRequest, IMapTo<Role>
{
    public string Id { get; set; } = null!;
    public string Name { get; init; } = null!;
    public List<string> Permissions { get; set; } = new();

    public sealed class Validator : AbstractValidator<UpdateRoleRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.Permissions).NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<UpdateRoleRequest>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Role> _roleRepository;

        public Handler(IMapper mapper, IRepository<Role> roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<Unit> Handle(UpdateRoleRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role is null)
                throw new NotFoundException($"Role {request.Id} does not exist");
            role = _mapper.Map(request, role);
            await _roleRepository.UpdateAsync(role, cancellationToken);
            return Unit.Value;
        }
    }
}

using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Roles.Queries;

public sealed class GetRoleByIdQuery : IRequest<RoleDto>
{
    public required string RoleId { get; init; }

    public sealed class Validator : AbstractValidator<GetRoleByIdQuery>
    {
        public Validator()
        {
            RuleFor(e => e.RoleId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public Handler(IMapper mapper, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken) ?? throw ExceptionFor<Role>.NotFound(r => r.Id, request.RoleId);
            return _mapper.Map<RoleDto>(role);
        }
    }
}

using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Roles.Queries;

public sealed class GetRolesResponse
{
    public List<RoleDto> Roles { get; init; } = new();
}

public sealed class GetRolesQuery : IRequest<GetRolesResponse>
{
    public List<string?>? Permissions { get; init; }

    public sealed class Specification : Specification<Role>
    {
        public Specification(List<string?>? permissions)
        {
            if (permissions is [_, ..])
                Query.Where(r => r.Permissions.Any(s => permissions.Any(t => t != null && s.Contains(t!))));
        }
    }

    public sealed class Handler : IRequestHandler<GetRolesQuery, GetRolesResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public Handler(IMapper mapper, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<GetRolesResponse> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.ListAsync(new Specification(request.Permissions), cancellationToken);

            return new GetRolesResponse { Roles = _mapper.Map<List<RoleDto>>(roles) };
        }
    }
}

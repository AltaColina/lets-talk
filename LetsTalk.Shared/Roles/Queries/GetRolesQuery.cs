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
            return new GetRolesResponse { Roles = _mapper.Map<List<RoleDto>>(await _roleRepository.ListAsync(cancellationToken)) };
        }
    }
}

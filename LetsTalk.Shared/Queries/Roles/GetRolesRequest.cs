using AutoMapper;
using LetsTalk.Dtos;
using LetsTalk.Interfaces;
using MediatR;
using LetsTalk.Models;

namespace LetsTalk.Queries.Roles;

public sealed class GetRolesResponse
{
    public List<RoleDto> Roles { get; init; } = new();
}

public sealed class GetRolesRequest : IRequest<GetRolesResponse>
{
    public sealed class Handler : IRequestHandler<GetRolesRequest, GetRolesResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Role> _roleRepository;

        public Handler(IMapper mapper, IRepository<Role> roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<GetRolesResponse> Handle(GetRolesRequest request, CancellationToken cancellationToken)
        {
            return new GetRolesResponse { Roles = _mapper.Map<List<RoleDto>>(await _roleRepository.ListAsync(cancellationToken)) };
        }
    }
}

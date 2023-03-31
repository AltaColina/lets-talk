using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;

namespace LetsTalk.Roles.Queries;

public sealed class GetRolesResponse
{
    public List<RoleDto> Roles { get; init; } = new();
}

public sealed class GetRolesQuery : IRequest<Response<GetRolesResponse>>
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

    public sealed class Handler : IRequestHandler<GetRolesQuery, Response<GetRolesResponse>>
    {
        private readonly IValidatorService<GetRolesQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public Handler(IValidatorService<GetRolesQuery> validator, IMapper mapper, IRoleRepository roleRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<Response<GetRolesResponse>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var roles = await _roleRepository.ListAsync(new Specification(request.Permissions), cancellationToken);

            return new GetRolesResponse { Roles = _mapper.Map<List<RoleDto>>(roles) };
        }
    }
}

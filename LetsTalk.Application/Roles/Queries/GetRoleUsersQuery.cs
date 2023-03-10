using Ardalis.Specification;
using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Roles.Queries;

public sealed class GetRoleUsersResponse
{
    public required List<UserDto> Users { get; init; }
}

public sealed class GetRoleUsersQuery : IRequest<GetRoleUsersResponse>
{
    public required string Id { get; init; }

    public sealed class Validator : AbstractValidator<GetRoleUsersQuery>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Specification : Specification<User>
    {
        public Specification(string roleId)
        {
            Query.Where(user => user.Roles.Contains(roleId));
        }
    }

    public sealed class Handler : IRequestHandler<GetRoleUsersQuery, GetRoleUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public Handler(IMapper mapper, IRoleRepository roleRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<GetRoleUsersResponse> Handle(GetRoleUsersQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role is null)
                throw ExceptionFor<Role>.NotFound(r => r.Id, request.Id);

            var users = await _userRepository.ListAsync(new Specification(request.Id), cancellationToken);
            return new GetRoleUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

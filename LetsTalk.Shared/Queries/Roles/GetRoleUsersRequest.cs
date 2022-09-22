using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using MediatR;
using FluentValidation;
using LetsTalk.Models;

namespace LetsTalk.Queries.Roles;

public sealed class GetRoleUsersResponse
{
    public List<UserDto> Users { get; init; } = new();
}

public sealed class GetRoleUsersRequest : IRequest<GetRoleUsersResponse>
{
    public string Id { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetRoleUsersRequest>
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

    public sealed class Handler : IRequestHandler<GetRoleUsersRequest, GetRoleUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;

        public Handler(IMapper mapper, IRepository<Role> roleRepository, IRepository<User> userRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<GetRoleUsersResponse> Handle(GetRoleUsersRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role is null)
                throw new NotFoundException($"Role {request.Id} does not exist");

            var users = await _userRepository.ListAsync(new Specification(request.Id), cancellationToken);
            return new GetRoleUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

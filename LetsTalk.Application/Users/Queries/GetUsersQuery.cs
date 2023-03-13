using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Users.Queries;

public sealed class GetUsersResponse
{
    public List<UserDto> Users { get; init; } = new();
}

public sealed class GetUsersQuery : IRequest<GetUsersResponse>
{
    public List<string?>? Roles { get; init; }

    public sealed class Specification : Specification<User>
    {
        public Specification(List<string?>? roles)
        {
            if (roles is [_, ..])
                Query.Where(u => u.Roles.Any(s => roles.Any(t => t != null && s.Contains(t!))));
        }
    }

    public sealed class Handler : IRequestHandler<GetUsersQuery, GetUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public Handler(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.ListAsync(new Specification(request.Roles), cancellationToken);

            return new GetUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

using AutoMapper;
using LetsTalk.Interfaces;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Users.Queries;

public sealed class GetUsersResponse
{
    public List<UserDto> Users { get; init; } = new();
}

public sealed class GetUsersQuery : IRequest<GetUsersResponse>
{
    public sealed class Handler : IRequestHandler<GetUsersQuery, GetUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;

        public Handler(IMapper mapper, IRepository<User> userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return new GetUsersResponse { Users = _mapper.Map<List<UserDto>>(await _userRepository.ListAsync(cancellationToken)) };
        }
    }
}

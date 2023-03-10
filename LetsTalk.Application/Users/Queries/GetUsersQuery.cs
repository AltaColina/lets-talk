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
            return new GetUsersResponse { Users = _mapper.Map<List<UserDto>>(await _userRepository.ListAsync(cancellationToken)) };
        }
    }
}

using AutoMapper;
using LetsTalk.Dtos;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Queries.Users;

public sealed class GetUsersResponse
{
    public List<UserDto> Users { get; init; } = new();
}

public sealed class GetUsersRequest : IRequest<GetUsersResponse>
{
    public sealed class Handler : IRequestHandler<GetUsersRequest, GetUsersResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;

        public Handler(IMapper mapper, IRepository<User> userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<GetUsersResponse> Handle(GetUsersRequest request, CancellationToken cancellationToken)
        {
            return new GetUsersResponse { Users = _mapper.Map<List<UserDto>>(await _userRepository.ListAsync(cancellationToken)) };
        }
    }
}

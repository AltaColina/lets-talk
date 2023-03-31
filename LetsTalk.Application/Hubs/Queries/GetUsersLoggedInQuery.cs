using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Hubs.Queries;

public sealed class GetUsersLoggedInResponse
{
    public required List<UserDto> Users { get; init; }
}

public sealed class GetUsersLoggedInQuery : IRequest<Response<GetUsersLoggedInResponse>>
{
    public sealed class Specification : Specification<User>
    {
        public Specification(IReadOnlyCollection<string> userIds)
        {
            Query.Where(user => userIds.Contains(user.Id));
        }
    }

    public sealed class Handler : IRequestHandler<GetUsersLoggedInQuery, Response<GetUsersLoggedInResponse>>
    {
        private readonly IValidatorService<GetUsersLoggedInQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IUserRepository _userRepository;

        public Handler(IValidatorService<GetUsersLoggedInQuery> validator, IMapper mapper, IHubConnectionManager connectionManager, IUserRepository userRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
        }

        public async Task<Response<GetUsersLoggedInResponse>> Handle(GetUsersLoggedInQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var users = await _userRepository.ListAsync(new Specification(_connectionManager.GetUserIds()), cancellationToken);
            return new GetUsersLoggedInResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

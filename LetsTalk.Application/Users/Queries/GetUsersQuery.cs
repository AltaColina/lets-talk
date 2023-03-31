using Ardalis.Specification;
using AutoMapper;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;

namespace LetsTalk.Users.Queries;

public sealed class GetUsersResponse
{
    public List<UserDto> Users { get; init; } = new();
}

public sealed class GetUsersQuery : IRequest<Response<GetUsersResponse>>
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

    public sealed class Handler : IRequestHandler<GetUsersQuery, Response<GetUsersResponse>>
    {
        private readonly IValidatorService<GetUsersQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public Handler(IValidatorService<GetUsersQuery> validator, IMapper mapper, IUserRepository userRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Response<GetUsersResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var users = await _userRepository.ListAsync(new Specification(request.Roles), cancellationToken);

            return new GetUsersResponse { Users = _mapper.Map<List<UserDto>>(users) };
        }
    }
}

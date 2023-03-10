using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Users.Queries;

public sealed class GetUserByIdQuery : IRequest<UserDto>
{
    public required string UserId { get; init; }

    public sealed class Validator : AbstractValidator<GetUserByIdQuery>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public Handler(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw ExceptionFor<User>.NotFound(r => r.Id, request.UserId);
            return _mapper.Map<UserDto>(user);
        }
    }
}

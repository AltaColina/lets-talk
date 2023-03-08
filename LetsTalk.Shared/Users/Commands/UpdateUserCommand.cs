using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Users.Commands;

public sealed class UpdateUserCommand : IRequest<UserDto>, IMapTo<User>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public HashSet<string> Roles { get; set; } = new();

    public sealed class Validator : AbstractValidator<UpdateUserCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Roles).NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<UpdateUserCommand, UserDto>

    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public Handler(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
                throw new NotFoundException($"User {request.Id} does not exist");
            user = _mapper.Map(request, user);
            await _userRepository.UpdateAsync(user, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }
    }
}

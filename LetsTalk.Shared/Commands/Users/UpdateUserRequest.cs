using AutoMapper;
using FluentValidation;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands.Users;

public sealed class UpdateUserRequest : IRequest<UserDto>, IMapTo<User>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public HashSet<string> Roles { get; set; } = new();

    public sealed class Validator : AbstractValidator<UpdateUserRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Roles).NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<UpdateUserRequest, UserDto>

    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;

        public Handler(IMapper mapper, IRepository<User> userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
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

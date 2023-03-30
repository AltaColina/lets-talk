using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using MediatR;
using System.Text.Json.Serialization;

namespace LetsTalk.Users.Commands;

public sealed class UpdateUserCommand : IRequest<UserDto>, IMapTo<User>
{
    [JsonIgnore]
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required HashSet<string> Roles { get; set; }

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
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw ExceptionFor<User>.NotFound(r => r.Id, request.Id);
            user = _mapper.Map(request, user);
            await _userRepository.UpdateAsync(user, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }
    }
}

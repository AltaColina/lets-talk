using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;
using System.Text.Json.Serialization;

namespace LetsTalk.Users.Commands;

public sealed class UpdateUserCommand : IRequest<Response<UserDto>>, IMapTo<User>
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

        public static Validator Instance { get; } = new();
    }

    public sealed class Handler : IRequestHandler<UpdateUserCommand, Response<UserDto>>
    {
        private readonly IValidatorService<UpdateUserCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public Handler(IValidatorService<UpdateUserCommand> validator, IMapper mapper, IUserRepository userRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Response<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
                return new NotFound();

            user = _mapper.Map(request, user);
            await _userRepository.UpdateAsync(user, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }
    }
}

using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Users.Commands;

public sealed class DeleteUserCommand : IRequest<Response<Success>>
{
    public required string Id { get; init; }

    public sealed class Validator : AbstractValidator<DeleteUserCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }

        public static Validator Instance { get; } = new();
    }

    public sealed class Handler : IRequestHandler<DeleteUserCommand, Response<Success>>
    {
        private readonly IValidatorService<DeleteUserCommand> _validator;
        private readonly IUserRepository _userRepository;

        public Handler(IValidatorService<DeleteUserCommand> validator, IUserRepository userRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
        }

        public async Task<Response<Success>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
                return new NotFound();

            await _userRepository.DeleteAsync(user, cancellationToken);

            return new Success();
        }
    }
}

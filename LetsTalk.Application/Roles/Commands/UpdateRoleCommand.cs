using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;
using System.Text.Json.Serialization;

namespace LetsTalk.Roles.Commands;

public sealed class UpdateRoleCommand : IRequest<Response<RoleDto>>, IMapTo<Role>
{
    [JsonIgnore]
    public required string Id { get; set; }

    public required string Name { get; init; }

    public required List<string> Permissions { get; init; }

    public sealed class Validator : AbstractValidator<UpdateRoleCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Name).NotEmpty();
            RuleFor(e => e.Permissions).NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<UpdateRoleCommand, Response<RoleDto>>
    {
        private readonly IValidatorService<UpdateRoleCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public Handler(IValidatorService<UpdateRoleCommand> validator, IMapper mapper, IRoleRepository roleRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<Response<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role is null)
                return new NotFound();

            role = _mapper.Map(request, role);
            await _roleRepository.UpdateAsync(role, cancellationToken);
            return _mapper.Map<RoleDto>(role);
        }
    }
}

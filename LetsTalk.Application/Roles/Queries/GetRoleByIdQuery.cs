using AutoMapper;
using FluentValidation;
using LetsTalk.Errors;
using LetsTalk.Repositories;
using LetsTalk.Services;
using MediatR;
using OneOf.Types;

namespace LetsTalk.Roles.Queries;

public sealed class GetRoleByIdQuery : IRequest<Response<RoleDto>>
{
    public required string RoleId { get; init; }

    public sealed class Validator : AbstractValidator<GetRoleByIdQuery>
    {
        public Validator()
        {
            RuleFor(e => e.RoleId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetRoleByIdQuery, Response<RoleDto>>
    {
        private readonly IValidatorService<GetRoleByIdQuery> _validator;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public Handler(IValidatorService<GetRoleByIdQuery> validator, IMapper mapper, IRoleRepository roleRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<Response<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return new Invalid(validation.ToDictionary());

            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null)
                return new NotFound();

            return _mapper.Map<RoleDto>(role);
        }
    }
}

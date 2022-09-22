using AutoMapper;
using FluentValidation;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Queries.Roles;

public sealed class GetRoleByIdRequest : IRequest<RoleDto>
{
    public string Id { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetRoleByIdRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetRoleByIdRequest, RoleDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Role> _roleRepository;

        public Handler(IMapper mapper, IRepository<Role> roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> Handle(GetRoleByIdRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (role is null)
                throw new NotFoundException($"Role {request.Id} does not exist");
            return _mapper.Map<RoleDto>(role);
        }
    }
}

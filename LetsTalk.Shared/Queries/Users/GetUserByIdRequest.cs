using AutoMapper;
using FluentValidation;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Queries.Users;

public sealed class GetUserByIdRequest : IRequest<UserDto>
{
    public string UserId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetUserByIdRequest>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetUserByIdRequest, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;

        public Handler(IMapper mapper, IRepository<User> userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new NotFoundException($"User {request.UserId} does not exist");
            return _mapper.Map<UserDto>(user);
        }
    }
}

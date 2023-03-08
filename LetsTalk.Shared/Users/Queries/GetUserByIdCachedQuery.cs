using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace LetsTalk.Users.Queries;
public sealed class GetUserByIdCachedQuery : IRequest<UserDto>
{
    public string UserId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetUserByIdCachedQuery>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetUserByIdCachedQuery, UserDto>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public Handler(IDistributedCache distributedCache, IMapper mapper, IUserRepository userRepository)
        {
            _distributedCache = distributedCache;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(GetUserByIdCachedQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"user_{request.UserId}";
            var user = await _distributedCache.GetStringAsync(cacheKey, token: cancellationToken) is string json
                ? JsonSerializer.Deserialize<UserDto>(json)
                : null;
            if (user is null)
            {
                var u = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (u is null)
                    throw new NotFoundException($"User {request.UserId} does not exist");
                user = _mapper.Map<UserDto>(u);
                await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user), token: cancellationToken);
            }
            return user;
        }
    }
}

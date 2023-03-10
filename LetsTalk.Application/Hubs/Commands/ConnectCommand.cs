﻿using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;

namespace LetsTalk.Hubs.Commands;

public sealed class ConnectCommand : IRequest<UserDto>
{
    public required string UserId { get; init; }
    public required string ConnectionId { get; init; }

    public sealed class Validator : AbstractValidator<ConnectCommand>
    {
        public Validator()
        {
            RuleFor(e => e.UserId).NotEmpty();
            RuleFor(e => e.ConnectionId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<ConnectCommand, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly IHubConnectionManager _connectionManager;
        private readonly IUserRepository _userRepository;

        public Handler(IMapper mapper, IHubConnectionManager connectionManager, IUserRepository userRepository)
        {
            _mapper = mapper;
            _connectionManager = connectionManager;
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(ConnectCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                throw ExceptionFor<User>.Unauthorized();
            _connectionManager.AddMapping(request.ConnectionId, user);

            return _mapper.Map<UserDto>(user);
        }
    }
}
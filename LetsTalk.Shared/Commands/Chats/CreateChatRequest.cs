using AutoMapper;
using FluentValidation;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands.Chats;

public sealed class CreateChatRequest : IRequest<ChatDto>, IMapTo<Chat>
{
    public string Id { get; set; } = null!;
    public string? Name { get; init; }

    public sealed class Validator : AbstractValidator<CreateChatRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<CreateChatRequest, ChatDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IMapper mapper, IRepository<Chat> chatRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
        }

        public async Task<ChatDto> Handle(CreateChatRequest request, CancellationToken cancellationToken)
        {
            if ((await _chatRepository.GetByIdAsync(request.Id, cancellationToken)) is not null)
                throw new ConflictException($"Chat {request.Id} already exists");
            var chat = _mapper.Map<Chat>(request);
            chat.Name ??= chat.Id;
            await _chatRepository.AddAsync(chat, cancellationToken);
            return _mapper.Map<ChatDto>(chat);
        }
    }
}

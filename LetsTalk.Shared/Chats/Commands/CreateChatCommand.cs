using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using MediatR;

namespace LetsTalk.Chats.Commands;

public sealed class CreateChatCommand : IRequest<ChatDto>, IMapTo<Chat>
{
    public string Id { get; set; } = null!;
    public string? Name { get; init; }

    public sealed class Validator : AbstractValidator<CreateChatCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<CreateChatCommand, ChatDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IMapper mapper, IRepository<Chat> chatRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
        }

        public async Task<ChatDto> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {
            if (await _chatRepository.GetByIdAsync(request.Id, cancellationToken) is not null)
                throw new ConflictException($"Chat {request.Id} already exists");
            var chat = _mapper.Map<Chat>(request);
            chat.Name ??= chat.Id;
            await _chatRepository.AddAsync(chat, cancellationToken);
            return _mapper.Map<ChatDto>(chat);
        }
    }
}

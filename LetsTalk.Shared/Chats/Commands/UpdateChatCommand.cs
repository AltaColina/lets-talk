using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Chats.Commands;

public sealed class UpdateChatCommand : IRequest<ChatDto>, IMapTo<Chat>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;

    public sealed class Validator : AbstractValidator<UpdateChatCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Name).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<UpdateChatCommand, ChatDto>
    {
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;

        public Handler(IMapper mapper, IChatRepository chatRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
        }

        public async Task<ChatDto> Handle(UpdateChatCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(request.Id, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.Id} does not exist");
            chat = _mapper.Map(request, chat);
            await _chatRepository.UpdateAsync(chat, cancellationToken);
            return _mapper.Map<ChatDto>(chat);
        }
    }
}
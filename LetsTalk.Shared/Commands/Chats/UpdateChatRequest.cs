using AutoMapper;
using FluentValidation;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands.Chats;

public sealed class UpdateChatRequest : IRequest<ChatDto>, IMapTo<Chat>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;

    public sealed class Validator : AbstractValidator<UpdateChatRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Name).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<UpdateChatRequest, ChatDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IMapper mapper, IRepository<Chat> chatRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
        }

        public async Task<ChatDto> Handle(UpdateChatRequest request, CancellationToken cancellationToken)
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
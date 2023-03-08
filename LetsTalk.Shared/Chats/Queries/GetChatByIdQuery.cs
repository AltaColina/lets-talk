using AutoMapper;
using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using MediatR;

namespace LetsTalk.Chats.Queries;

public sealed class GetChatByIdQuery : IRequest<ChatDto>
{
    public string ChatId { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetChatByIdQuery>
    {
        public Validator()
        {
            RuleFor(e => e.ChatId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetChatByIdQuery, ChatDto>
    {
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;

        public Handler(IMapper mapper, IChatRepository chatRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
        }

        public async Task<ChatDto> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.ChatId} does not exist");
            return _mapper.Map<ChatDto>(chat);
        }
    }
}
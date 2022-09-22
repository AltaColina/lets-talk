using AutoMapper;
using FluentValidation;
using LetsTalk.Dtos;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Queries.Chats;

public sealed class GetChatByIdRequest : IRequest<ChatDto>
{
    public string Id { get; init; } = null!;

    public sealed class Validator : AbstractValidator<GetChatByIdRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<GetChatByIdRequest, ChatDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IMapper mapper, IRepository<Chat> chatRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
        }

        public async Task<ChatDto> Handle(GetChatByIdRequest request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(request.Id, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.Id} does not exist");
            return _mapper.Map<ChatDto>(chat);
        }
    }
}
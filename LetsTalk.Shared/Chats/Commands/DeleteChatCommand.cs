using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using MediatR;

namespace LetsTalk.Chats.Commands;

public sealed class DeleteChatCommand : IRequest
{
    public string Id { get; init; } = null!;

    public sealed class Validator : AbstractValidator<DeleteChatCommand>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteChatCommand>
    {
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IRepository<Chat> chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task Handle(DeleteChatCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(request.Id, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.Id} does not exist");
            await _chatRepository.DeleteAsync(chat, cancellationToken);
        }
    }
}
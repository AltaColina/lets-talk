using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
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
        private readonly IChatRepository _chatRepository;

        public Handler(IChatRepository chatRepository)
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
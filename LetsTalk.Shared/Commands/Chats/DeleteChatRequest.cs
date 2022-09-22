using FluentValidation;
using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Commands.Chats;

public sealed class DeleteChatRequest : IRequest
{
    public string Id { get; init; } = null!;

    public sealed class Validator : AbstractValidator<DeleteChatRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteChatRequest>
    {
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IRepository<Chat> chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<Unit> Handle(DeleteChatRequest request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(request.Id, cancellationToken);
            if (chat is null)
                throw new NotFoundException($"Chat {request.Id} does not exist");
            await _chatRepository.DeleteAsync(chat, cancellationToken);

            return Unit.Value;
        }
    }
}
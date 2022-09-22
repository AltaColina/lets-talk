using AutoMapper;
using LetsTalk.Dtos;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;

namespace LetsTalk.Queries.Chats;

public sealed class GetChatsResponse
{
    public List<ChatDto> Chats { get; init; } = null!;
}

public sealed class GetChatsRequest : IRequest<GetChatsResponse>
{
    public sealed class Handler : IRequestHandler<GetChatsRequest, GetChatsResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IMapper mapper, IRepository<Chat> chatRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
        }

        public async Task<GetChatsResponse> Handle(GetChatsRequest request, CancellationToken cancellationToken)
        {
            return new GetChatsResponse { Chats = _mapper.Map<List<ChatDto>>(await _chatRepository.ListAsync(cancellationToken)) };
        }
    }
}

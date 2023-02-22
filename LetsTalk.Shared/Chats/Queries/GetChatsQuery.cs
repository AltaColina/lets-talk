using AutoMapper;
using LetsTalk.Interfaces;
using MediatR;

namespace LetsTalk.Chats.Queries;

public sealed class GetChatsResponse
{
    public List<ChatDto> Chats { get; init; } = null!;
}

public sealed class GetChatsQuery : IRequest<GetChatsResponse>
{
    public sealed class Handler : IRequestHandler<GetChatsQuery, GetChatsResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Chat> _chatRepository;

        public Handler(IMapper mapper, IRepository<Chat> chatRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
        }

        public async Task<GetChatsResponse> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            return new GetChatsResponse { Chats = _mapper.Map<List<ChatDto>>(await _chatRepository.ListAsync(cancellationToken)) };
        }
    }
}

using MediatR;

namespace LetsTalk.Models;

public sealed partial class GetChatRequest : IRequest<GetChatResponse> { }
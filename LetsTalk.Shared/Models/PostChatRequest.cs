using MediatR;

namespace LetsTalk.Models;

public sealed partial class PostChatRequest : IRequest<PostChatResponse> { }
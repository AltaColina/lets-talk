using MediatR;

namespace LetsTalk.Models;

public sealed partial class RegisterRequest : IRequest<RegisterResponse> { }
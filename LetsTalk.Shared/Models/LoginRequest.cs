using MediatR;

namespace LetsTalk.Models;

public sealed partial class LoginRequest : IRequest<LoginResponse> { }
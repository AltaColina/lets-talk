using LetsTalk.Errors;
using OneOf;

namespace LetsTalk;

[GenerateOneOf]
public sealed partial class Response<TSuccess> : OneOfBase<TSuccess, OneOf.Types.NotFound, AlreadyExists, Invalid> { }

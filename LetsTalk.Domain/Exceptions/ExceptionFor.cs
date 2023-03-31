using System.Linq.Expressions;

namespace LetsTalk.Exceptions;

public static class ExceptionFor<T>
{
    public static ConflictException AlreadyExists<TMember>(string expression, TMember value)
    {
        return new ConflictException($"{typeof(T).Name}: {expression} '{value}' already exists");
    }

    public static ConflictException AlreadyExists<TMember>(Expression<Func<T, TMember>> expression, TMember value)
    {
        return new ConflictException($"{typeof(T).Name}: {(expression.Body as MemberExpression)?.Member.Name} '{value}' already exists");
    }

    public static NotFoundException NotFound<TMember>(string expression, TMember value)
    {
        return new NotFoundException($"{typeof(T).Name}: {expression} '{value}' not found");
    }

    public static NotFoundException NotFound<TMember>(Expression<Func<T, TMember>> expression, TMember value)
    {
        return new NotFoundException($"{typeof(T).Name}: {(expression.Body as MemberExpression)?.Member.Name} '{value}' not found");
    }

    public static InvalidOperationException Invalid(string key)
    {
        return new InvalidOperationException($"{typeof(T).Name}: Invalid {key}");
    }

    public static InvalidOperationException Invalid(string key, object? value)
    {
        return new InvalidOperationException($"{typeof(T).Name}: Invalid {key} '{value}'");
    }

    public static InvalidOperationException Invalid(string key, IEnumerable<object?> values)
    {
        return new InvalidOperationException($"{typeof(T).Name}: Invalid {key} [{String.Join(',', values.Select(v => $"'{v}'"))}]");
    }

    public static ForbiddenException Forbidden()
    {
        return new ForbiddenException($"{typeof(T).Name}: forbidden");
    }

    public static UnauthorizedException Unauthorized()
    {
        return new UnauthorizedException($"{typeof(T).Name}: unauthorized");
    }
}
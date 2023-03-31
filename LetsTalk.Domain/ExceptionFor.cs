using System.Linq.Expressions;

namespace LetsTalk;

public static class ExceptionFor<T>
{
    public static InvalidOperationException AlreadyExists<TMember>(string expression, TMember value)
    {
        return new InvalidOperationException($"{typeof(T).Name}: {expression} '{value}' already exists");
    }

    public static InvalidOperationException AlreadyExists<TMember>(Expression<Func<T, TMember>> expression, TMember value)
    {
        return new InvalidOperationException($"{typeof(T).Name}: {(expression.Body as MemberExpression)?.Member.Name} '{value}' already exists");
    }

    public static InvalidOperationException NotFound<TMember>(string expression, TMember value)
    {
        return new InvalidOperationException($"{typeof(T).Name}: {expression} '{value}' not found");
    }

    public static InvalidOperationException NotFound<TMember>(Expression<Func<T, TMember>> expression, TMember value)
    {
        return new InvalidOperationException($"{typeof(T).Name}: {(expression.Body as MemberExpression)?.Member.Name} '{value}' not found");
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

    public static InvalidOperationException Forbidden()
    {
        return new InvalidOperationException($"{typeof(T).Name}: forbidden");
    }

    public static InvalidOperationException Unauthorized()
    {
        return new InvalidOperationException($"{typeof(T).Name}: unauthorized");
    }
}
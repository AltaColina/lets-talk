using Serilog.Core;
using Serilog.Events;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace LetsTalk.Services;

public sealed class SensitiveDataDestructuringPolicy : IDestructuringPolicy
{
    private static readonly ConcurrentDictionary<Type, Func<object, ILogEventPropertyValueFactory, LogEventPropertyValue>?> Cache = new();

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        var destruct = Cache.GetOrAdd(value.GetType(), CreateCacheEntry);
        if (destruct is null)
        {
            result = null;
            return false;
        }
        else
        {
            result = destruct.Invoke(value, propertyValueFactory);
            return true;
        }
    }

    static Func<object, ILogEventPropertyValueFactory, LogEventPropertyValue>? CreateCacheEntry(Type type)
    {
        var properties = type.GetProperties();

        if (!properties.Any(p => p.GetCustomAttribute<SensitiveDataAttribute>() is not null))
            return null;

        var getters = GetGetMethods(type, properties);

        var attributes = properties
            .Select(p => p.GetCustomAttribute<SensitiveDataAttribute>() is SensitiveDataAttribute Attribute ? new { p.Name, Attribute } : null)
            .Where(o => o is not null)
            .ToDictionary(o => o!.Name, o => o!.Attribute!);

        return (o, f) =>
        {
            var structured = new List<LogEventProperty>();
            foreach (var (name, getter) in getters)
            {
                var value = getter.Invoke(o)!;

                if (attributes.TryGetValue(name, out var attribute))
                {
                    structured.Add(new(name, new ScalarValue(attribute.FormatValue(value))));
                }
                else
                {
                    structured.Add(new(name, f.CreatePropertyValue(value, true)));
                }
            }

            return new StructureValue(structured, type.Name);
        };

        static Dictionary<string, Func<object, object?>> GetGetMethods(Type type, IEnumerable<PropertyInfo> properties)
        {
            var getMethods = new Dictionary<string, Func<object, object?>>();
            foreach (var property in properties)
            {
                var eParam = Expression.Parameter(typeof(object));
                var eProp = Expression.Convert(Expression.Property(Expression.Convert(eParam, type), property.Name), typeof(object));
                var elambda = Expression.Lambda<Func<object, object?>>(eProp, eParam);
                getMethods[property.Name] = elambda.Compile();
            }

            return getMethods;
        }
    }

}

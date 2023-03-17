using Serilog.Core;
using Serilog.Events;

namespace LetsTalk;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SensitiveDataAttribute : Attribute, ILogEventPropertyValueFactory
{
    private readonly char _maskChar = '*';
    private readonly ScalarValue _mask = new("***");

    public char MaskChar
    {
        get => _maskChar;
        init
        {
            _maskChar = value;
            _mask = new(new string(value, 3));
        }
    }

    public bool PreserveLength { get; init; }

    public int ShowFirst { get; init; }

    public int ShowLast { get; init; }

    private readonly record struct CreateState(SensitiveDataAttribute Attribute, string Value);

    private LogEventPropertyValue FormatValue(string? value)
    {
        if (String.IsNullOrEmpty(value))
            return new ScalarValue(value);

        if (ShowFirst + ShowLast > 0)
        {
            var v = String.Create(value.Length, new CreateState(this, value), FormatSpan);
            return new ScalarValue(v);
        }
        else
        {
            return PreserveLength
                ? new ScalarValue(new string(_maskChar, value.Length))
                : _mask;
        }

        static void FormatSpan(Span<char> span, CreateState state)
        {
            var first = state.Attribute.ShowFirst;
            var last = state.Attribute.ShowLast;

            if (first + last > span.Length)
            {
                state.Value.CopyTo(span);
            }
            else
            {
                ReadOnlySpan<char> value = state.Value;
                char mask = state.Attribute._maskChar;

                value[..first].CopyTo(span[..first]);
                value[^last..].CopyTo(span[^last..]);
                span[first..^last].Fill(mask);
            }
        }
    }

    public LogEventPropertyValue CreatePropertyValue(object value, bool destructureObjects = false) => value switch
    {
        IEnumerable<string> strings => new SequenceValue(strings.Select(FormatValue)),
        string @string => FormatValue(@string),
        _ => new ScalarValue(null),
    };
}
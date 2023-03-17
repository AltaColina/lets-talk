using Serilog.Events;

namespace LetsTalk;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SensitiveDataAttribute : Attribute
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

    private LogEventPropertyValue FormatValue(string? value)
    {
        if (String.IsNullOrEmpty(value))
            return new ScalarValue(value);

        return PreserveLength
            ? new ScalarValue(new string(_maskChar, value.Length))
            : _mask;
    }

    public LogEventPropertyValue FormatValue(object? value) => value switch
    {
        IEnumerable<string> strings => new SequenceValue(strings.Select(FormatValue)),
        string @string => FormatValue(@string),
        _ => new ScalarValue(null),
    };
}
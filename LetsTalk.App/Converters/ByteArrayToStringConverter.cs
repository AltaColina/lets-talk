﻿using CommunityToolkit.Maui.Converters;
using System.Globalization;
using System.Text;

namespace LetsTalk.App.Converters;

public sealed class ByteArrayToStringConverter : BaseConverter<byte[], string>
{
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    public override string ConvertFrom(byte[] value, CultureInfo? culture)
    {
        return value is null ? null! : Encoding.GetString(value);
    }

    public override byte[] ConvertBackTo(string value, CultureInfo? culture)
    {
        return value is null ? null! : Encoding.GetBytes(value);
    }
}

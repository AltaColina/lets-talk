﻿using LetsTalk.Messaging;

namespace LetsTalk.Models;

public sealed class ContentMessageTemplateSelector : DataTemplateSelector
{
    public required DataTemplate TextPlain { get; set; }
    public required DataTemplate Image { get; set; }
    public required DataTemplate Default { get; set; }


    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var message = item as ContentMessage;
        if (message is not null)
        {
            switch (message.ContentType)
            {
                case MimeType.Text.Plain:
                    return TextPlain;
                case MimeType.Image.Bmp:
                case MimeType.Image.Jpeg:
                case MimeType.Image.Png:
                case MimeType.Image.Tiff:
                    return Image;
            }
        }

        return Default;
    }
}

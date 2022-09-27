namespace LetsTalk.App.Models;

public sealed class MessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextMessageTemplate { get; set; } = null!;
    public DataTemplate MediaMessageTemplate { get; set; } = null!;
    public DataTemplate DefaultTemplate { get; set; } = null!;


    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is not null)
        {

        }

        return DefaultTemplate;
    }
}

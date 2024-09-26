using iTextSharp.text;
using PdfDream.Interfaces;

namespace PdfDream.Tags;

internal readonly struct TitleTag(string content, bool isBold) : INonAutoCloseTag
{
    public readonly string TagName { get; } = "Title";
    public readonly string Content { get; } = content;
    public readonly bool IsBold { get; } = isBold;

    public void Render(ref Document doc)
    {
        Font titleFont;

        if (IsBold)
        {
            titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
        }
        else
        {
            titleFont = FontFactory.GetFont(FontFactory.HELVETICA, 18);
        }

        var paragraph = new Paragraph(Content, titleFont)
        {
            Alignment = Element.ALIGN_CENTER
        };

        doc.Add(paragraph);
    }
}
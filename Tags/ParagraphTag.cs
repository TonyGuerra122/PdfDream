using iTextSharp.text;
using PdfDream.Interfaces;

namespace PdfDream.Tags;

public readonly struct ParagraphTag(string content, int? alignment, float? fontSize) : INonAutoCloseTag
{
    public readonly string TagName { get; } = "P";
    public readonly string Content { get; } = content;
    public readonly int Alignment { get; } = alignment ?? Element.ALIGN_LEFT;
    public readonly float FontSize { get; } = fontSize ?? 8f;

    public void Render(ref Document doc)
    {
        var font = FontFactory.GetFont(FontFactory.HELVETICA, FontSize);

        var paragraph = new Paragraph(Content, font);

        doc.Add(paragraph);
    }
}

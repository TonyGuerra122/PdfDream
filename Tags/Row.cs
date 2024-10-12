using iTextSharp.text;
using PdfDream.Interfaces;

namespace PdfDream.Tags;

internal readonly struct Row(List<Cell> content, BaseColor? backgroundColor = null) : INonAutoCloseTag
{
    public readonly string TagName { get; } = "Row";
    public readonly List<Cell> Content { get; } = content;
    public readonly BaseColor? Color { get; } = backgroundColor;

    public void Render(ref Document doc)
    {
        throw new NotImplementedException();
    }
}

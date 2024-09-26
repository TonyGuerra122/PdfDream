using iTextSharp.text;

namespace PdfDream.Interfaces;

internal interface IAutoCloseTag
{
    string TagName { get; }
    void Render(ref Document doc);
}

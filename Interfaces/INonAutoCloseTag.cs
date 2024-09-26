using iTextSharp.text;

namespace PdfDream.Interfaces;

internal interface INonAutoCloseTag
{
    string TagName { get; }
    void Render(ref Document doc);
}

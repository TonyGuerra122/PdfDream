using iTextSharp.text;
using PdfDream.Interfaces;

namespace PdfDream.Tags;

public readonly struct ImgTag(string src, int? alignment, float? width, float? height) : IAutoCloseTag
{
    public string TagName { get; } = "Img";
    public string Src { get; } = src;
    public int Alignment { get; } = alignment ?? Element.ALIGN_CENTER;
    public float Width { get; } = width ?? 500f;
    public float Height { get; } = height ?? 500f;

    public void Render(ref Document doc)
    {
        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), Src);

        if (!File.Exists(imgPath)) throw new FileNotFoundException($"Image {imgPath} not founded.");

        var img = Image.GetInstance(imgPath);

        img.ScaleToFit(Width, Height);

        img.Alignment = Alignment;

        doc.Add(img);
    }
}


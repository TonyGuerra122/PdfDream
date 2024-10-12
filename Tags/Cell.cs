using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfDream.Errors;
using PdfDream.Interfaces;
using System.Text.RegularExpressions;

namespace PdfDream.Tags;

internal readonly struct Cell : INonAutoCloseTag
{
	public string? Content { get; }  // Para texto
	public string? ImageSrc { get; } // Para imagens
	public float? Width { get; }
	public float? Height { get; }
	public int Alignment { get; }
	public bool IsImage { get; }

	public readonly string TagName { get; } = "Cell";

	internal static readonly string[] separator = ["\n"];

	public Cell(string content, int alignment = Element.ALIGN_CENTER)
	{
		Content = content;
		ImageSrc = null;
		Width = null;
		Height = null;
		Alignment = alignment;
		IsImage = false;
	}

	public Cell(string imageSrc, float? width, float? height, int alignment = Element.ALIGN_CENTER)
	{
		ImageSrc = imageSrc;
		Content = null;
		Width = width;
		Height = height;
		Alignment = alignment;
		IsImage = true;
	}

	public void Render(ref Document doc)
	{
		if (IsImage)
		{
			if (string.IsNullOrEmpty(ImageSrc)) throw new PdfArgumentException(nameof(ImageSrc));

			var imgTag = new ImgTag(ImageSrc, Alignment, Width, Height);

			imgTag.Render(ref doc);
		}
		else
		{
			if (Content != null)
			{
				PdfPCell cell = new()
				{
					HorizontalAlignment = Alignment,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					Border = PdfPCell.NO_BORDER
				};

				string[] lines = Content.Split(separator, StringSplitOptions.None);

				foreach (string line in lines)
				{
					cell.AddElement(new Paragraph(line));
				}

				PdfPTable table = new(1);
				table.AddCell(cell);

				doc.Add(table);
			}
		}
	}

}

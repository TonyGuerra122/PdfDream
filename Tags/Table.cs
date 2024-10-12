using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfDream.Interfaces;

namespace PdfDream.Tags;

internal readonly struct Table(List<Row> content) : INonAutoCloseTag
{
	public readonly string TagName { get; } = "Table";
	public readonly List<Row> Content { get; } = content;

	public void Render(ref Document doc)
	{
		if (Content.Count == 0) return;

		int numColumns = Content[0].Content.Count;

		var pdfTable = new PdfPTable(numColumns)
		{
			WidthPercentage = 100
		};

		foreach (var row in Content)
		{
			foreach (var cell in row.Content)
			{
				PdfPCell pdfPCell;

				if (cell.IsImage)
				{

					var imgTag = new ImgTag(cell.ImageSrc ?? "", cell.Alignment, cell.Width, cell.Height);

					pdfPCell = new PdfPCell(imgTag.RenderToImage())
					{
						HorizontalAlignment = cell.Alignment,
						VerticalAlignment = Element.ALIGN_MIDDLE,
						Padding = 5
					};
				}
				else
				{
					string[]? lines = cell.Content?.Split('\n', StringSplitOptions.None);

					Paragraph paragraph = [];

					foreach (string line in lines ?? [])
					{
						paragraph.Add(new Phrase(line));
						paragraph.Add(Chunk.Newline);
					}

					pdfPCell = new PdfPCell(paragraph)
					{
						HorizontalAlignment = cell.Alignment,
						VerticalAlignment = Element.ALIGN_CENTER,
						Padding = 5
					};
				}

				if (row.Color != null)
				{
					pdfPCell.BackgroundColor = row.Color;
				}

				pdfTable.AddCell(pdfPCell);
			}
		}

		doc.Add(pdfTable);
	}
}

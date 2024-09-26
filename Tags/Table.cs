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

		// Número de colunas baseado na primeira linha
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

				// Verificar se a célula contém uma imagem
				if (cell.IsImage)
				{
					// Carregar a imagem e definir propriedades
					var img = Image.GetInstance(cell.ImageSrc);
					img.ScaleToFit(cell.Width ?? 100f, cell.Height ?? 100f);
					img.Alignment = cell.Alignment;

					// Adicionar a imagem dentro da célula
					pdfPCell = new PdfPCell(img)
					{
						HorizontalAlignment = cell.Alignment,
						VerticalAlignment = Element.ALIGN_MIDDLE,
						Padding = 5
					};
				}
				else
				{
					// Dividir o conteúdo em linhas separadas por quebra de linha '\n'
					string[]? lines = cell.Content?.Split('\n', StringSplitOptions.None);

					// Criar um parágrafo para armazenar múltiplas linhas
					Paragraph paragraph = [];

					foreach (string line in lines ?? [])
					{
						paragraph.Add(new Phrase(line));
						paragraph.Add(Chunk.Newline);  // Adicionar uma nova linha entre parágrafos
					}

					// Criar a célula e adicionar o parágrafo
					pdfPCell = new PdfPCell(paragraph)
					{
						HorizontalAlignment = cell.Alignment, // Alinhar o conteúdo
						VerticalAlignment = Element.ALIGN_CENTER,
						Padding = 5
					};
				}

				// Definir cor de fundo, se presente
				if (row.Color != null)
				{
					pdfPCell.BackgroundColor = row.Color;
				}

				pdfTable.AddCell(pdfPCell);
			}
		}

		// Adicionar a tabela ao documento
		doc.Add(pdfTable);
	}
}

using iTextSharp.text;
using PdfDream.Interfaces;
using System.Text.RegularExpressions;

namespace PdfDream.Tags;

public readonly partial struct ImgTag(string src, int? alignment, float? width, float? height) : IAutoCloseTag
{
	public string TagName { get; } = "Img";
	public string Src { get; } = src;
	public int Alignment { get; } = alignment ?? Element.ALIGN_CENTER;
	public float Width { get; } = width ?? 500f;
	public float Height { get; } = height ?? 500f;

	public void Render(ref Document doc)
	{
		Image img;

		if (IsBase64String(Src))
		{
			byte[] imageBytes = Convert.FromBase64String(ExtractBase64Data(Src));
			img = Image.GetInstance(imageBytes);
		}
		else
		{
			string imagePath = Path.Combine(Directory.GetCurrentDirectory(), Src);

			if (!File.Exists(imagePath)) throw new FileNotFoundException($"Imagem {imagePath} não encontrada");

			img = Image.GetInstance(imagePath);
		}

		img.ScaleToFit(Width, Height);
		img.Alignment = Alignment;

		doc.Add(img);
	}

	public Image RenderToImage()
	{
		Image img;

		if (IsBase64String(Src))
		{
			byte[] imageBytes = Convert.FromBase64String(ExtractBase64Data(Src));
			img = Image.GetInstance(imageBytes);
		}
		else
		{
			string imagePath = Path.Combine(Directory.GetCurrentDirectory(), Src);

			if (!File.Exists(imagePath)) throw new FileNotFoundException($"Imagem {imagePath} não encontrada");

			img = Image.GetInstance(imagePath);
		}

		img.ScaleToFit(Width, Height);
		img.Alignment = Alignment;

		return img;
	}


	private static bool IsBase64String(string src) => src.Contains("data:image", StringComparison.OrdinalIgnoreCase);

	private static string ExtractBase64Data(string base64String)
	{
		int index = base64String.IndexOf("data:image");

		if (index != -1)
		{
			string fileBase64 = base64String[index..];

			return Base64Regex().Replace(fileBase64, string.Empty); ;
		}

		throw new ArgumentException("Prefixo 'data:image' não encontrado na string Base64.");
	}

	[GeneratedRegex(@"^data:image\/[a-zA-Z]+;base64,")]
	private static partial Regex Base64Regex();
}


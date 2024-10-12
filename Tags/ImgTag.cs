﻿using iTextSharp.text;
using PdfDream.Interfaces;
using System.Text.RegularExpressions;

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

	private static bool IsBase64String(string src) => src.StartsWith("data:image", StringComparison.OrdinalIgnoreCase);

	private static string ExtractBase64Data(string base64String) => Regex.Replace(base64String, @"^data:image\/[a-zA-Z]+;base64", string.Empty);
}


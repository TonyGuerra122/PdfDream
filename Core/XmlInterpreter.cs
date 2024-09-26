using iTextSharp.text;
using PdfDream.Errors;
using PdfDream.Tags;
using System.Text;
using System.Xml;

namespace PdfDream.Core;

internal class XmlInterpreter
{
    private readonly XmlNodeList _documentNodes;

    public XmlInterpreter(string xmlContent)
    {
        if (string.IsNullOrEmpty(xmlContent)) throw new ArgumentNullException(nameof(xmlContent), "O conteúdo XML não pode ser nulo ou vazio.");
        
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlContent);

        _documentNodes = xmlDoc.GetElementsByTagName("PdfDocument");
        if (_documentNodes.Count == 0) throw new Errors.PdfException("Raiz do XML 'PdfDocument' não encontrada.");
    }

    private static int ExtractAlignment(XmlNode node)
    {
        string? alignmentValue = node.Attributes?["alignment"]?.Value;

        if (string.IsNullOrEmpty(alignmentValue)) return Element.ALIGN_LEFT;

        return alignmentValue.ToLower() switch
        {
            "left" => Element.ALIGN_LEFT,
            "right" => Element.ALIGN_RIGHT,
            "center" => Element.ALIGN_CENTER,
            "justify" => Element.ALIGN_JUSTIFIED,
            _ => throw new PdfArgumentException($"Invalid alignment {alignmentValue}")
        };
    }

    private static float? ExtractWidth(XmlNode node)
    {
        string? widthValue = node.Attributes?["width"]?.Value;

        if (string.IsNullOrEmpty(widthValue)) return null;

        if (float.TryParse(widthValue, out var width))
        {
            return width;
        }

        throw new PdfArgumentException($"Invalid width: {widthValue}");
    }

    private static float? ExtractHeight(XmlNode node)
    {
        string? heightValue = node.Attributes?["height"]?.Value;

        if (string.IsNullOrEmpty(heightValue)) return null;

        if (float.TryParse(heightValue, out var height))
        {
            return height;
        }

        throw new PdfArgumentException($"Invalid height: {heightValue}");
    }

    private static float? ExtractFontSize(XmlNode node)
    {
        string? fontValue = node.Attributes?["fontSize"]?.Value;

        if (string.IsNullOrEmpty(fontValue)) return null;

        if (float.TryParse(fontValue, out var fontSize))
        {
            return fontSize;
        }

        throw new PdfArgumentException($"Invalid font size: {fontValue}");
    }

    private static BaseColor ConvertHexToBaseColor(string hex)
    {
        hex = hex.Replace("#", "");

        if (hex.Length == 6)
        {
			int r = int.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return new BaseColor(r, g, b);
        }
        else return BaseColor.White;
    }

    public void IncrementDocument(ref Document doc)
    {
        foreach (XmlNode documentNode in _documentNodes)
        {
            foreach (XmlNode childNode in documentNode.ChildNodes)
            {
                if (childNode.Name.Equals("Img", StringComparison.OrdinalIgnoreCase))
                {
                    string src = childNode.Attributes?["src"]?.Value ?? throw new PdfDream.Errors.PdfNullTagException("A tag 'Img' está sem o atributo 'src'.");

                    int alignment = ExtractAlignment(childNode);

                    float? width = ExtractWidth(childNode);

                    float? height = ExtractHeight(childNode);

                    var imgTag = new ImgTag(src, alignment, width, height);

                    imgTag.Render(ref doc);
                }
                else if (childNode.Name.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    string titleContent = childNode.InnerText ?? throw new PdfNullTagException("The 'Title' tag is empty.");

                    bool isBold = childNode.Attributes?["bold"]?.Value?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

                    var titleTag = new TitleTag(titleContent, isBold);

                    titleTag.Render(ref doc);
                }
                else if (childNode.Name.Equals("P", StringComparison.OrdinalIgnoreCase))
                {
                    string paragraphContent = childNode.InnerText ?? throw new PdfNullTagException("The 'Paragraph' tag is empty.");

                    int alignment = ExtractAlignment(childNode);

                    float fontSize = ExtractFontSize(childNode) ?? 8f;

                    var paragraphTag = new ParagraphTag(paragraphContent, alignment, fontSize);

                    paragraphTag.Render(ref doc);

                    doc.Add(Chunk.Newline);
                }
                else if (childNode.Name.Equals("Table", StringComparison.OrdinalIgnoreCase))
                {
                    List<Tags.Row> rows = [];

                    foreach (XmlNode rowNode in childNode.ChildNodes)
                    {
                        if (rowNode.Name.Equals("Row", StringComparison.OrdinalIgnoreCase))
                        {
                            List<Tags.Cell> cells = [];

                            string? color = rowNode.Attributes?["color"]?.Value;

                            BaseColor? rowBgColor = null;

                            if (!string.IsNullOrEmpty(color))
                            {
                                rowBgColor = ConvertHexToBaseColor(color);
                            }

							foreach (XmlNode cellNode in rowNode.ChildNodes)
							{
								if (cellNode.Name.Equals("Cell", StringComparison.OrdinalIgnoreCase))
								{
									XmlNode? imgNode = cellNode.SelectSingleNode("Img");
									if (imgNode != null)
									{
										string imgSrc = Path.Combine(Directory.GetCurrentDirectory(), imgNode.Attributes?["src"]?.Value ?? throw new PdfNullTagException("A tag 'Img' está sem o atributo 'src'."));
										float? imgWidth = ExtractWidth(imgNode);
										float? imgHeight = ExtractHeight(imgNode);
										int imgAlignment = ExtractAlignment(imgNode);

										cells.Add(new Tags.Cell(imgSrc, imgWidth, imgHeight, imgAlignment));
									}
									else
									{
										var paragraphNodes = cellNode.SelectNodes("P");
										var cellContentBuilder = new StringBuilder();

										if (paragraphNodes != null && paragraphNodes.Count > 0)
										{
											foreach (XmlNode paragraphNode in paragraphNodes)
											{
												string paragraphContent = paragraphNode.InnerText ?? string.Empty;
												cellContentBuilder.AppendLine(paragraphContent);
											}
										}
										else
										{
											string cellContent = cellNode.InnerText ?? string.Empty;
											cellContentBuilder.Append(cellContent);
										}

										cells.Add(new Tags.Cell(cellContentBuilder.ToString()));
									}
								}
							}


							rows.Add(new Tags.Row(cells, rowBgColor));
                        }
                    }

                    var table = new Tags.Table(rows);
                    table.Render(ref doc);
                } else if(childNode.Name.Equals("BR", StringComparison.OrdinalIgnoreCase))
                {
                    doc.Add(Chunk.Newline);
                }
                else if (childNode.Name.Equals("#comment", StringComparison.OrdinalIgnoreCase)) continue;
                else
                {
                    throw new PdfDream.Errors.PdfNullTagException($"Tag '{childNode.Name}' não suportada.");
                }
            }
        }
    }
}

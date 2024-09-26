using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfDream.Core;

namespace PdfDream;

public class PdfCreator(string xmlContent)
{
    private readonly string _xmlContent = xmlContent;

    public const float PX_PER_MM = 72 / 25.2F;

    public string GeneratePreviewDocument()
    {
        var pdf = new Document(PageSize.A4, 15 * PX_PER_MM, 15 * PX_PER_MM, 15 * PX_PER_MM, 20 * PX_PER_MM);
        string tempFileName = Path.Combine(Path.GetTempPath(), $"document_preview_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        var tempFile = new FileStream(tempFileName, FileMode.Create);
        var writer = PdfWriter.GetInstance(pdf, tempFile);

        try
        {
            pdf.Open();

            var xmlInterpreter = new XmlInterpreter(_xmlContent);

            xmlInterpreter.IncrementDocument(ref pdf);
        }
        catch (Exception ex)
        {
            throw new Errors.PdfException(ex.Message);
        }
        finally
        {
            pdf.Close();
            writer.Close();
        }

        return tempFileName;
    }
}

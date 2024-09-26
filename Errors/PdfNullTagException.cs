namespace PdfDream.Errors;

internal class PdfNullTagException(string argName) : ArgumentException($"O argumento '{argName}' não pode ser nulo ou vazio.")
{
}

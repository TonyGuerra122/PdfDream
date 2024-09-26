# PdfDream

PdfDream � uma biblioteca C# que facilita a gera��o de PDFs a partir de arquivos XML usando o iText7. Ela permite criar PDFs de maneira simples e flex�vel, aplicando fontes personalizadas, alinhamentos, estilos e muito mais.

## Instala��o

Voc� pode instalar o pacote PDFDoc via NuGet:

```bash
dotnet add package PdfDream
```
Ou, no Visual Studio, procure por PDFDoc na interface de gerenciamento de pacotes NuGet.

## Uso
### Exemplo de XML para gerar um PDF

Aqui est� um exemplo de como usar o PDFDoc para gerar um PDF a partir de um arquivo XML:
```bash
<PdfDocument>
    <Text font="custom" bold="true">T�tulo em negrito com fonte personalizada!</Text>
    <Table>
        <Row>
            <Cell alignment="center" color="red" bold="true">Header 1</Cell>
            <Cell alignment="center" color="red" bold="true">Header 2</Cell>
        </Row>
        <Row>
            <Cell>Data 1</Cell>
            <Cell>Data 2</Cell>
        </Row>
        <Row>
            <Cell>Data 3</Cell>
            <Cell>Data 4</Cell>
        </Row>
    </Table>
</PdfDocument>
```

## Exemplo de c�digo C# usando PDFDoc

Aqui est� um exemplo de como integrar a biblioteca PDFDoc ao seu projeto C#:
```bash
using iText.Kernel.Font;
using PDFDoc;

namespace ExampleApp
{
    public class Program
    {
        public static void Main()
        {
            // Exemplo de XML
            string xmlContent = @"
            <PdfDocument>
                <Text font='custom' bold='true'>T�tulo em negrito com fonte personalizada!</Text>
                <Table>
                    <Row>
                        <Cell alignment='center' color='red' bold='true'>Header 1</Cell>
                        <Cell alignment='center' color='red' bold='true'>Header 2</Cell>
                    </Row>
                    <Row>
                        <Cell>Data 1</Cell>
                        <Cell>Data 2</Cell>
                    </Row>
                    <Row>
                        <Cell>Data 3</Cell>
                        <Cell>Data 4</Cell>
                    </Row>
                </Table>
            </PdfDocument>";

            // Instancia o PdfBuilder com o XML
            var builder = new PdfBuilder(xmlContent)
            {
                CustomFont = PdfFontFactory.CreateFont("Roboto-Medium.ttf", PdfEncodings.WINANSI, true)
            };

            // Gera o PDF como byte[] e salva em um arquivo
            byte[] pdfBytes = builder.BuildPdf();
            System.IO.File.WriteAllBytes("output.pdf", pdfBytes);

            Console.WriteLine("PDF gerado com sucesso!");
        }
    }
}
```

## Personaliza��es

### Voc� pode personalizar diversos aspectos do PDF atrav�s do XML:

-   Text: Para adicionar um t�tulo ou par�grafo com fontes e estilos.
-   Table: Para criar tabelas com cabe�alhos e linhas de dados.
-   Cell: Cada c�lula da tabela pode ter alinhamento, cor de fundo e texto em negrito.
-   Fontes: Defina fontes personalizadas com o atributo font="custom".

### Atributos Suportados

    font: Define a fonte usada no texto. Use custom para fontes personalizadas.
    bold: Define se o texto ou c�lula deve estar em negrito.
    alignment: Define o alinhamento do texto (left, center, right, justify).
    color: Define a cor de fundo da c�lula (em ingl�s ou formato hexadecimal).
    
## Licen�a

Este projeto est� licenciado sob os termos da licen�a MIT. Veja o arquivo LICENSE para mais detalhes.
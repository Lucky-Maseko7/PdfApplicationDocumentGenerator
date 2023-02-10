using suffusive.uveitis.Dependencies.Model;

namespace suffusive.uveitis.Dependencies.Interfaces
{
    public interface IPdfGenerator
    {
        PdfDocument GenerateFromHtml(string view, PdfOptions pdfOptions);
    }
}
using Recruitment.Interfaces;
using UglyToad.PdfPig;

namespace Recruitment.Services;

public class PdfReaderService : IPdfReaderService
{
    public Task<string> ExtractTextAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return Task.FromResult(string.Empty);

        try
        {
            using var document = PdfDocument.Open(filePath);
            var text = new System.Text.StringBuilder();
            
            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }
            
            return Task.FromResult(text.ToString());
        }
        catch
        {
            return Task.FromResult(string.Empty);
        }
    }
}

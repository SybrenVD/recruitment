namespace Recruitment.Interfaces;

public interface IPdfReaderService
{
    Task<string> ExtractTextAsync(string filePath);
}

public interface IAIService
{
    Task<string> AnalyzeCVAsync(string cvText, string candidateName, string location);
}

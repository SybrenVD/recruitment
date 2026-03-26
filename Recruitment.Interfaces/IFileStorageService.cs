namespace Recruitment.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string container);
    Task<Stream?> GetFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
}

using Recruitment.Interfaces;

namespace Recruitment.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public FileStorageService(IConfiguration configuration)
    {
        _basePath = configuration.GetValue<string>("FileStorage:BasePath") ?? "wwwroot/uploads";
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string container)
    {
        var directory = Path.Combine(_basePath, container);
        Directory.CreateDirectory(directory);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var filePath = Path.Combine(directory, uniqueFileName);
        var relativePath = Path.Combine(container, uniqueFileName);

        using var outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream);

        return relativePath.Replace("\\", "/");
    }

    public async Task<Stream?> GetFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        
        if (!File.Exists(fullPath))
            return null;

        var memoryStream = new MemoryStream();
        using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public Task DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        return Task.FromResult(File.Exists(fullPath));
    }
}

using Recruitment.Interfaces;

namespace Recruitment.Services;

public class CandidateService : ICandidateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;

    public CandidateService(IUnitOfWork unitOfWork, IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    public async Task<Entities.Candidate?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Candidates.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Entities.Candidate>> GetAllAsync()
    {
        return await _unitOfWork.Candidates.GetAllAsync();
    }

    public async Task<Entities.Candidate> CreateAsync(Entities.Candidate candidate)
    {
        candidate.CreatedAt = DateTime.UtcNow;
        return await _unitOfWork.Candidates.AddAsync(candidate);
    }

    public async Task UpdateAsync(Entities.Candidate candidate)
    {
        await _unitOfWork.Candidates.UpdateAsync(candidate);
    }

    public async Task DeleteAsync(int id)
    {
        var candidate = await GetByIdAsync(id);
        if (candidate?.CVFilePath != null)
        {
            await _fileStorage.DeleteFileAsync(candidate.CVFilePath);
        }
        await _unitOfWork.Candidates.DeleteAsync(id);
    }

    public async Task<Entities.Candidate?> GetByEmailAsync(string email)
    {
        var candidates = await _unitOfWork.Candidates.FindAsync(c => c.Email == email);
        return candidates.FirstOrDefault();
    }

    public async Task<string> UploadCVAsync(int candidateId, Stream fileStream, string fileName)
    {
        var candidate = await GetByIdAsync(candidateId);
        if (candidate == null)
            throw new InvalidOperationException("Candidate not found");

        if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) &&
            !fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only PDF and DOCX files are allowed");
        }

        if (candidate.CVFilePath != null)
        {
            await _fileStorage.DeleteFileAsync(candidate.CVFilePath);
        }

        var filePath = await _fileStorage.SaveFileAsync(fileStream, fileName, "cvs");
        candidate.CVFilePath = filePath;
        await UpdateAsync(candidate);

        return filePath;
    }

    public async Task<Stream?> GetCVAsync(int candidateId)
    {
        var candidate = await GetByIdAsync(candidateId);
        if (candidate?.CVFilePath == null)
            return null;

        return await _fileStorage.GetFileAsync(candidate.CVFilePath);
    }
}

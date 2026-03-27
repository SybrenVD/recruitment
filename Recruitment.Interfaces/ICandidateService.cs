using Recruitment.Entities;

namespace Recruitment.Interfaces;

public interface ICandidateService
{
    Task<Candidate?> GetByIdAsync(int id);
    Task<IEnumerable<Candidate>> GetAllAsync();
    Task<Candidate> CreateAsync(Candidate candidate);
    Task UpdateAsync(Candidate candidate);
    Task DeleteAsync(int id);
    Task<Candidate?> GetByEmailAsync(string email);
    Task<string> UploadCVAsync(int candidateId, Stream fileStream, string fileName);
    Task<Stream?> GetCVAsync(int candidateId);
}

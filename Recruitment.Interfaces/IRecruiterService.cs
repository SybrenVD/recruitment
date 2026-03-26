using Recruitment.Entities;

namespace Recruitment.Interfaces;

public interface IRecruiterService
{
    Task<Recruiter?> GetByIdAsync(int id);
    Task<IEnumerable<Recruiter>> GetAllAsync();
    Task<Recruiter> CreateAsync(Recruiter recruiter);
    Task UpdateAsync(Recruiter recruiter);
    Task DeleteAsync(int id);
    Task<Recruiter?> GetByEmailAsync(string email);
    Task<IEnumerable<Job>> GetJobsAsync(int recruiterId);
}

using Recruitment.Entities;

namespace Recruitment.Interfaces;

public interface IJobService
{
    Task<Job?> GetByIdAsync(int id);
    Task<IEnumerable<Job>> GetAllAsync(string? searchTerm = null, string? location = null, string? experienceLevel = null);
    Task<Job> CreateAsync(Job job);
    Task UpdateAsync(Job job);
    Task DeleteAsync(int id);
    Task<IEnumerable<JobSkill>> GetSkillsAsync(int jobId);
    Task AddSkillAsync(int jobId, JobSkill jobSkill);
    Task RemoveSkillAsync(int jobId, int skillId);
    Task<IEnumerable<JobMatch>> GetMatchesAsync(int jobId);
}

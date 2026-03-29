using Recruitment.Entities;

namespace Recruitment.Interfaces;

public interface IJobService
{
    Task<Job?> GetByIdAsync(int id);
    Task<IEnumerable<Job>> GetAllAsync(string? searchTerm = null, string? location = null, string? experienceLevel = null);
    Task<IEnumerable<Job>> GetAvailableForCandidateAsync(int candidateId);
    Task<Job> CreateAsync(Job job);
    Task<Job> CreateWithSkillsAsync(Job job, List<(string name, int level)> skills);
    Task UpdateAsync(Job job);
    Task DeleteAsync(int id);
    Task<IEnumerable<JobSkill>> GetSkillsAsync(int jobId);
    Task AddSkillAsync(int jobId, JobSkill jobSkill);
    Task RemoveSkillAsync(int jobId, int skillId);
    Task<IEnumerable<JobMatch>> GetMatchesAsync(int jobId);
    Task<Skill> GetOrCreateSkillAsync(string skillName);
}

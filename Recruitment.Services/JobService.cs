using Microsoft.EntityFrameworkCore;
using Recruitment.Interfaces;

namespace Recruitment.Services;

public class JobService : IJobService
{
    private readonly IUnitOfWork _unitOfWork;

    public JobService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Entities.Job?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Jobs.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Entities.Job>> GetAllAsync(string? searchTerm = null, string? location = null, string? experienceLevel = null)
    {
        var jobs = await _unitOfWork.Jobs.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            jobs = jobs.Where(j => 
                (j.Title != null && j.Title.ToLower().Contains(searchTerm)) ||
                (j.Description != null && j.Description.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            jobs = jobs.Where(j => j.Location != null && j.Location.ToLower().Contains(location.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(experienceLevel))
        {
            jobs = jobs.Where(j => j.ExperienceLevel != null && j.ExperienceLevel.ToLower() == experienceLevel.ToLower());
        }

        return jobs;
    }

    public async Task<Entities.Job> CreateAsync(Entities.Job job)
    {
        job.CreatedAt = DateTime.UtcNow;
        return await _unitOfWork.Jobs.AddAsync(job);
    }

    public async Task UpdateAsync(Entities.Job job)
    {
        await _unitOfWork.Jobs.UpdateAsync(job);
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.Jobs.DeleteAsync(id);
    }

    public async Task<IEnumerable<Entities.JobSkill>> GetSkillsAsync(int jobId)
    {
        return await _unitOfWork.JobSkills.FindAsync(js => js.JobId == jobId);
    }

    public async Task AddSkillAsync(int jobId, Entities.JobSkill jobSkill)
    {
        jobSkill.JobId = jobId;
        await _unitOfWork.JobSkills.AddAsync(jobSkill);
    }

    public async Task RemoveSkillAsync(int jobId, int skillId)
    {
        var jobSkill = (await _unitOfWork.JobSkills.FindAsync(js => js.JobId == jobId && js.SkillId == skillId)).FirstOrDefault();
        if (jobSkill != null)
        {
            await _unitOfWork.JobSkills.DeleteAsync(jobSkill.Id);
        }
    }

    public async Task<IEnumerable<Entities.JobMatch>> GetMatchesAsync(int jobId)
    {
        return await _unitOfWork.JobMatches.FindAsync(jm => jm.JobId == jobId);
    }
}

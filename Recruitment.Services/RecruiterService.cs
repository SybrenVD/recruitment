using Recruitment.Interfaces;

namespace Recruitment.Services;

public class RecruiterService : IRecruiterService
{
    private readonly IUnitOfWork _unitOfWork;

    public RecruiterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Entities.Recruiter?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Recruiters.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Entities.Recruiter>> GetAllAsync()
    {
        return await _unitOfWork.Recruiters.GetAllAsync();
    }

    public async Task<Entities.Recruiter> CreateAsync(Entities.Recruiter recruiter)
    {
        recruiter.CreatedAt = DateTime.UtcNow;
        return await _unitOfWork.Recruiters.AddAsync(recruiter);
    }

    public async Task UpdateAsync(Entities.Recruiter recruiter)
    {
        await _unitOfWork.Recruiters.UpdateAsync(recruiter);
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.Recruiters.DeleteAsync(id);
    }

    public async Task<Entities.Recruiter?> GetByEmailAsync(string email)
    {
        var recruiters = await _unitOfWork.Recruiters.FindAsync(r => r.Email == email);
        return recruiters.FirstOrDefault();
    }

    public async Task<IEnumerable<Entities.Job>> GetJobsAsync(int recruiterId)
    {
        var jobs = await _unitOfWork.Jobs.FindAsync(j => j.RecruiterId == recruiterId);
        return jobs;
    }
}

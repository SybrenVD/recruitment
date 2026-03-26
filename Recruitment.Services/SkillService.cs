using Recruitment.Interfaces;

namespace Recruitment.Services;

public class SkillService : ISkillService
{
    private readonly IUnitOfWork _unitOfWork;

    public SkillService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Entities.Skill?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Skills.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Entities.Skill>> GetAllAsync()
    {
        return await _unitOfWork.Skills.GetAllAsync();
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        var skills = await GetAllAsync();
        return skills
            .Where(s => !string.IsNullOrWhiteSpace(s.Category))
            .Select(s => s.Category!)
            .Distinct()
            .OrderBy(c => c);
    }

    public async Task<Entities.Skill> CreateAsync(Entities.Skill skill)
    {
        return await _unitOfWork.Skills.AddAsync(skill);
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.Skills.DeleteAsync(id);
    }
}

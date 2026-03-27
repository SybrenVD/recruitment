using Recruitment.Entities;

namespace Recruitment.Interfaces;

public interface ISkillService
{
    Task<Skill?> GetByIdAsync(int id);
    Task<IEnumerable<Skill>> GetAllAsync();
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<Skill> CreateAsync(Skill skill);
    Task DeleteAsync(int id);
}

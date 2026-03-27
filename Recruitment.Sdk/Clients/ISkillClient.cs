using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Sdk.Clients
{
    public interface ISkillClient
    {
        Task<IEnumerable<SkillResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SkillResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SkillResponse?> CreateAsync(CreateSkillRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}

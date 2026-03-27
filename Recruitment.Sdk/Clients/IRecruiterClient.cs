using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Sdk.Clients
{
    public interface IRecruiterClient
    {
        Task<IEnumerable<RecruiterResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<RecruiterResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<RecruiterResponse?> CreateAsync(CreateRecruiterRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}

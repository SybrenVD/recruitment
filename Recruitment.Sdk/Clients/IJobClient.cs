using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Sdk.Clients
{
    public interface IJobClient
    {
        Task<IEnumerable<JobResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<JobResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<JobResponse?> CreateAsync(CreateJobRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}

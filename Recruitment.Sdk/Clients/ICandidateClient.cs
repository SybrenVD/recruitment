using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Sdk.Clients
{
    public interface ICandidateClient
    {
        Task<IEnumerable<CandidateResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CandidateResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CandidateResponse?> CreateAsync(CreateCandidateRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}

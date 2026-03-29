using Recruitment.Entities;

namespace Recruitment.Interfaces;

public interface IMatchingService
{
    Task<JobMatch> CalculateMatchAsync(int candidateId, int jobId);
    Task<IEnumerable<JobMatch>> GetSuggestionsForCandidateAsync(int candidateId, int limit = 10);
    Task<IEnumerable<JobMatch>> GetSuggestionsForJobAsync(int jobId, int limit = 10);
    Task<bool> ProcessSwipeAsync(int candidateId, int jobId, bool isLike);
    Task<bool> ProcessRecruiterSwipeAsync(int candidateId, int jobId, bool isLike);
    Task<IEnumerable<JobMatch>> GetMutualMatchesAsync(int userId, bool isCandidate);
    Task<IEnumerable<JobMatch>> GetAvailableForRecruiterAsync(int recruiterId, int jobId);
}

using Recruitment.Entities;

namespace Recruitment.Interfaces;

public interface IMatchingService
{
    Task<JobMatch> CalculateMatchAsync(int candidateId, int jobId);
    Task<IEnumerable<JobMatch>> GetSuggestionsForCandidateAsync(int candidateId, int limit = 10);
    Task<IEnumerable<JobMatch>> GetSuggestionsForJobAsync(int jobId, int limit = 10);
    Task ProcessSwipeAsync(int candidateId, int jobId, bool isLike);
    Task<IEnumerable<JobMatch>> GetMutualMatchesAsync(int userId, bool isCandidate);
}

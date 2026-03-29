using Recruitment.Interfaces;

namespace Recruitment.Services;

public class MatchingService : IMatchingService
{
    private readonly IUnitOfWork _unitOfWork;

    public MatchingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Entities.JobMatch> CalculateMatchAsync(int candidateId, int jobId)
    {
        var candidateSkills = await _unitOfWork.CandidateSkills.FindAsync(cs => cs.CandidateId == candidateId);
        var jobSkills = await _unitOfWork.JobSkills.FindAsync(js => js.JobId == jobId);

        int totalScore = 0;
        int maxScore = jobSkills.Count() * 5 * 100;
        var skillGaps = new List<string>();

        foreach (var jobSkill in jobSkills)
        {
            var candidateSkill = candidateSkills.FirstOrDefault(cs => cs.SkillId == jobSkill.SkillId);
            if (candidateSkill != null)
            {
                var diff = candidateSkill.Level - jobSkill.RequiredLevel;
                var skillScore = Math.Max(0, (5 - Math.Abs(diff)) * 100 / 5);
                totalScore += skillScore * jobSkill.Weight;
            }
            else
            {
                skillGaps.Add($"Missing required skill level {jobSkill.RequiredLevel}");
            }
        }

        var matchScore = maxScore > 0 ? (totalScore * 100) / maxScore : 0;

        var existingMatch = (await _unitOfWork.JobMatches.FindAsync(
            jm => jm.CandidateId == candidateId && jm.JobId == jobId)).FirstOrDefault();

        if (existingMatch != null)
        {
            existingMatch.MatchScore = matchScore;
            existingMatch.SkillGap = string.Join(", ", skillGaps);
            await _unitOfWork.SaveChangesAsync();
            return existingMatch;
        }

        var newMatch = new Entities.JobMatch
        {
            CandidateId = candidateId,
            JobId = jobId,
            MatchScore = matchScore,
            SkillGap = string.Join(", ", skillGaps),
            CreatedAt = DateTime.UtcNow
        };

        return await _unitOfWork.JobMatches.AddAsync(newMatch);
    }

    public async Task<IEnumerable<Entities.JobMatch>> GetSuggestionsForCandidateAsync(int candidateId, int limit = 10)
    {
        var allJobs = await _unitOfWork.Jobs.GetAllAsync();
        var matches = new List<Entities.JobMatch>();

        foreach (var job in allJobs)
        {
            var match = await CalculateMatchAsync(candidateId, job.Id);
            matches.Add(match);
        }

        return matches.OrderByDescending(m => m.MatchScore).Take(limit);
    }

    public async Task<IEnumerable<Entities.JobMatch>> GetSuggestionsForJobAsync(int jobId, int limit = 10)
    {
        var allCandidates = await _unitOfWork.Candidates.GetAllAsync();
        var matches = new List<Entities.JobMatch>();

        foreach (var candidate in allCandidates)
        {
            var match = await CalculateMatchAsync(candidate.Id, jobId);
            matches.Add(match);
        }

        return matches.OrderByDescending(m => m.MatchScore).Take(limit);
    }

    public async Task<bool> ProcessSwipeAsync(int candidateId, int jobId, bool isLike)
    {
        var existingMatch = (await _unitOfWork.JobMatches.FindAsync(
            jm => jm.CandidateId == candidateId && jm.JobId == jobId)).FirstOrDefault();

        if (existingMatch == null)
        {
            existingMatch = new Entities.JobMatch
            {
                CandidateId = candidateId,
                JobId = jobId,
                MatchScore = 0,
                IsLikedByCandidate = isLike,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.JobMatches.AddAsync(existingMatch);
        }
        else
        {
            existingMatch.IsLikedByCandidate = isLike;
        }

        await _unitOfWork.SaveChangesAsync();

        if (isLike && existingMatch.IsLikedByRecruiter == true)
        {
            return true;
        }
        
        return false;
    }

    public async Task<bool> ProcessRecruiterSwipeAsync(int candidateId, int jobId, bool isLike)
    {
        var existingMatch = (await _unitOfWork.JobMatches.FindAsync(
            jm => jm.CandidateId == candidateId && jm.JobId == jobId)).FirstOrDefault();

        if (existingMatch == null)
        {
            existingMatch = new Entities.JobMatch
            {
                CandidateId = candidateId,
                JobId = jobId,
                MatchScore = 0,
                IsLikedByRecruiter = isLike,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.JobMatches.AddAsync(existingMatch);
        }
        else
        {
            existingMatch.IsLikedByRecruiter = isLike;
        }

        await _unitOfWork.SaveChangesAsync();

        if (isLike && existingMatch.IsLikedByCandidate == true)
        {
            return true;
        }
        
        return false;
    }

    public async Task<IEnumerable<Entities.JobMatch>> GetMutualMatchesAsync(int userId, bool isCandidate)
    {
        if (isCandidate)
        {
            var matches = (await _unitOfWork.JobMatches.FindAsync(jm => 
                jm.CandidateId == userId && 
                jm.IsLikedByCandidate == true && 
                jm.IsLikedByRecruiter == true)).ToList();
            
            foreach (var match in matches)
            {
                var job = await _unitOfWork.Jobs.GetByIdAsync(match.JobId);
                if (job != null)
                {
                    var recruiter = await _unitOfWork.Recruiters.GetByIdAsync(job.RecruiterId);
                    match.Job = job;
                    match.Job.Recruiter = recruiter;
                }
            }
            
            return matches;
        }
        else
        {
            var allMatches = await _unitOfWork.JobMatches.FindAsync(jm => 
                jm.Job != null && jm.Job.RecruiterId == userId);
            var mutualMatches = allMatches.Where(jm => 
                jm.IsLikedByCandidate == true && 
                jm.IsLikedByRecruiter == true).ToList();
            
            foreach (var match in mutualMatches)
            {
                match.Candidate = await _unitOfWork.Candidates.GetByIdAsync(match.CandidateId);
            }
            
            return mutualMatches;
        }
    }

    public async Task<IEnumerable<Entities.JobMatch>> GetAvailableForRecruiterAsync(int recruiterId, int jobId)
    {
        var job = await _unitOfWork.Jobs.GetByIdAsync(jobId);
        if (job == null || job.RecruiterId != recruiterId)
            return Enumerable.Empty<Entities.JobMatch>();

        var allMatches = await _unitOfWork.JobMatches.FindAsync(jm => jm.JobId == jobId && jm.IsLikedByRecruiter == null);
        
        var result = new List<Entities.JobMatch>();
        foreach (var match in allMatches)
        {
            await CalculateMatchAsync(match.CandidateId, jobId);
            var updatedMatch = await _unitOfWork.JobMatches.FindAsync(jm => jm.CandidateId == match.CandidateId && jm.JobId == jobId);
            var fullMatch = updatedMatch.FirstOrDefault();
            if (fullMatch != null && fullMatch.IsLikedByRecruiter == null)
            {
                var candidate = await _unitOfWork.Candidates.GetByIdAsync(fullMatch.CandidateId);
                fullMatch.Candidate = candidate;
                result.Add(fullMatch);
            }
        }
        
        return result.OrderByDescending(m => m.MatchScore);
    }
}

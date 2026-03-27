using Recruitment.Interfaces;

namespace Recruitment.Services;

public class CVAnalysisService : ICVAnalysisService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;

    public CVAnalysisService(IUnitOfWork unitOfWork, IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    public async Task<Entities.CVAnalysis> AnalyzeCVAsync(int candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(candidateId);
        if (candidate == null)
            throw new InvalidOperationException("Candidate not found");

        var existingAnalysis = await _unitOfWork.CVAnalyses.FindAsync(a => a.CandidateId == candidateId);
        var analysis = existingAnalysis.FirstOrDefault();

        if (analysis == null)
        {
            analysis = new Entities.CVAnalysis
            {
                CandidateId = candidateId,
                Summary = $"CV analysis for {candidate.FirstName} {candidate.LastName}",
                ExperienceLevel = "Mid-Level",
                Strengths = "Strong technical background",
                Weaknesses = "Limited management experience",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.CVAnalyses.AddAsync(analysis);
        }

        return analysis;
    }

    public async Task<Entities.CVAnalysis?> GetAnalysisAsync(int candidateId)
    {
        var analyses = await _unitOfWork.CVAnalyses.FindAsync(a => a.CandidateId == candidateId);
        return analyses.FirstOrDefault();
    }
}

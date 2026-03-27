using Recruitment.Entities;

namespace Recruitment.Interfaces;

public interface ICVAnalysisService
{
    Task<CVAnalysis> AnalyzeCVAsync(int candidateId);
    Task<CVAnalysis?> GetAnalysisAsync(int candidateId);
}

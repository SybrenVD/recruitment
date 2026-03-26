namespace Recruitment.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Entities.Candidate> Candidates { get; }
    IRepository<Entities.Recruiter> Recruiters { get; }
    IRepository<Entities.Job> Jobs { get; }
    IRepository<Entities.Skill> Skills { get; }
    IRepository<Entities.JobSkill> JobSkills { get; }
    IRepository<Entities.CandidateSkill> CandidateSkills { get; }
    IRepository<Entities.JobMatch> JobMatches { get; }
    IRepository<Entities.InterviewQuestion> InterviewQuestions { get; }
    IRepository<Entities.CVAnalysis> CVAnalyses { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

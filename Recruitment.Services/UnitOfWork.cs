using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Recruitment.Data;
using Recruitment.Interfaces;

namespace Recruitment.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly RecruitmentDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private IRepository<Entities.Candidate>? _candidates;
    private IRepository<Entities.Recruiter>? _recruiters;
    private IRepository<Entities.Job>? _jobs;
    private IRepository<Entities.Skill>? _skills;
    private IRepository<Entities.JobSkill>? _jobSkills;
    private IRepository<Entities.CandidateSkill>? _candidateSkills;
    private IRepository<Entities.JobMatch>? _jobMatches;
    private IRepository<Entities.InterviewQuestion>? _interviewQuestions;
    private IRepository<Entities.CVAnalysis>? _cvAnalyses;

    public UnitOfWork(RecruitmentDbContext context)
    {
        _context = context;
    }

    public IRepository<Entities.Candidate> Candidates =>
        _candidates ??= new Repository<Entities.Candidate>(_context);

    public IRepository<Entities.Recruiter> Recruiters =>
        _recruiters ??= new Repository<Entities.Recruiter>(_context);

    public IRepository<Entities.Job> Jobs =>
        _jobs ??= new Repository<Entities.Job>(_context);

    public IRepository<Entities.Skill> Skills =>
        _skills ??= new Repository<Entities.Skill>(_context);

    public IRepository<Entities.JobSkill> JobSkills =>
        _jobSkills ??= new Repository<Entities.JobSkill>(_context);

    public IRepository<Entities.CandidateSkill> CandidateSkills =>
        _candidateSkills ??= new Repository<Entities.CandidateSkill>(_context);

    public IRepository<Entities.JobMatch> JobMatches =>
        _jobMatches ??= new Repository<Entities.JobMatch>(_context);

    public IRepository<Entities.InterviewQuestion> InterviewQuestions =>
        _interviewQuestions ??= new Repository<Entities.InterviewQuestion>(_context);

    public IRepository<Entities.CVAnalysis> CVAnalyses =>
        _cvAnalyses ??= new Repository<Entities.CVAnalysis>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

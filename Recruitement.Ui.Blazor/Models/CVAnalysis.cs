namespace Recruitement.Ui.Blazor.Models;

public class CVAnalysis
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public string? Summary { get; set; }
    public string? ExperienceLevel { get; set; }
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InterviewQuestion
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int CandidateId { get; set; }
    public string Question { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

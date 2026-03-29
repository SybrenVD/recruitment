namespace Recruitement.Ui.Blazor.Models;

public class JobMatch
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public int JobId { get; set; }
    public int MatchScore { get; set; }
    public string? SkillGap { get; set; }
    public DateTime CreatedAt { get; set; }
    public Candidate? Candidate { get; set; }
    public Job? Job { get; set; }
}

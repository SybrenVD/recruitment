namespace Recruitement.Ui.Blazor.Models;

public class Candidate
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Location { get; set; }
    public string? CVFilePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CandidateSkillDto>? CandidateSkills { get; set; }
}

public class CandidateSkillDto
{
    public int SkillId { get; set; }
    public string? SkillName { get; set; }
    public int Level { get; set; }
}

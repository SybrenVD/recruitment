namespace Recruitement.Ui.Blazor.Models;

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ExperienceLevel { get; set; }
    public string? EducationLevel { get; set; }
    public int RecruiterId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<JobSkillDto>? JobSkills { get; set; }
    public RecruiterInfo? Recruiter { get; set; }
}

public class RecruiterInfo
{
    public int Id { get; set; }
    public string? Company { get; set; }
}

public class JobSkillDto
{
    public int SkillId { get; set; }
    public SkillInfo? Skill { get; set; }
    public int RequiredLevel { get; set; }
    public int Weight { get; set; }
    
    public string? SkillName => Skill?.SkillName;
}

public class SkillInfo
{
    public int Id { get; set; }
    public string? SkillName { get; set; }
    public string? Category { get; set; }
}

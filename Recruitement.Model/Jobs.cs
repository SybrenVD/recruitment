using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitement.Model
{
    [Table(nameof(Jobs))]
    public class Jobs
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string Location { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? EducationLevel { get; set; }
        public int RecruiterId { get; set; }
        public Recruiters? Recruiter { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

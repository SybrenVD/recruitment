using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitement.Model
{
    [Table(nameof(CVAnalysis))]
    public class CVAnalysis
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public Candidates? Candidates { get; set; }
        public string? Summary { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

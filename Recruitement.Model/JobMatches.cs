using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitement.Model
{
    [Table(nameof(JobMatches))]
    public class JobMatches
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Jobs? Job { get; set; }
        public int CandidateId { get; set; }
        public Candidates? Candidate { get; set; }
        [Column(TypeName = "decimal(5,4)")]
        public decimal MatchScore { get; set; }
        public string? SkillGap { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

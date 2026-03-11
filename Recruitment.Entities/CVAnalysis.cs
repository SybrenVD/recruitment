using System;

namespace Recruitment.Entities
{
    public class CVAnalysis
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string? Summary { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public DateTime CreatedAt { get; set; }

        public Candidate Candidate { get; set; } = null!;
    }
}

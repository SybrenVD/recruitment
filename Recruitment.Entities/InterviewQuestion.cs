using System;

namespace Recruitment.Entities
{
    public class InterviewQuestion
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }
        public string Question { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Job Job { get; set; } = null!;
        public Candidate Candidate { get; set; } = null!;
    }
}

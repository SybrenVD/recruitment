using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitement.Model
{
    [Table(nameof(InterviewQuestions))]
    public class InterviewQuestions
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Jobs? Jobs { get; set; }
        public int CandidateId { get; set; }
        public Candidates? Candidate { get; set; }
        public required string Question { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

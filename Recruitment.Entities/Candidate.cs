using System;
using System.Collections.Generic;

namespace Recruitment.Entities
{
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

        public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
        public ICollection<JobMatch> JobMatches { get; set; } = new List<JobMatch>();
        public ICollection<InterviewQuestion> InterviewQuestions { get; set; } = new List<InterviewQuestion>();
        public CVAnalysis? CVAnalysis { get; set; }
    }
}

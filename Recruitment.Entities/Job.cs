using System;
using System.Collections.Generic;

namespace Recruitment.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? EducationLevel { get; set; }
        public int RecruiterId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Recruiter Recruiter { get; set; } = null!;
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<JobMatch> JobMatches { get; set; } = new List<JobMatch>();
        public ICollection<InterviewQuestion> InterviewQuestions { get; set; } = new List<InterviewQuestion>();
    }
}

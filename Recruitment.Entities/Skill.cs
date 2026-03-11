using System.Collections.Generic;

namespace Recruitment.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? Category { get; set; }

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
    }
}

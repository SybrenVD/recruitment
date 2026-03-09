using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitement.Model
{
    [Table (nameof(CandidateSkills))] 
    public class CandidateSkills
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public Candidates? Candidate { get; set; }
        public int SkillId { get; set; }
        public Skills? Skill { get; set; }
        public int Level { get; set; }
    }
}

namespace Recruitment.Entities
{
    public class CandidateSkill
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int SkillId { get; set; }
        public int Level { get; set; }

        public Candidate Candidate { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}

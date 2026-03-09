using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitement.Model
{
    [Table(nameof(JobSkills))]
    public class JobSkills
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Jobs? Job { get; set; }
        public int SkillId { get; set; }
        public Skills? Skill { get; set; }
        public int RequiredLevel { get; set; }
        public int Weight { get; set; }
    }
}

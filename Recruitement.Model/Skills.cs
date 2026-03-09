using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitement.Model
{
    [Table(nameof(Skills))]
    public class Skills
    {
        public int Id { get; set; }
        public required string SkillName { get; set; }
        public required string Category { get; set; }
    }
}

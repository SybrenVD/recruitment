using System.ComponentModel.DataAnnotations;

namespace Recruitment.Requests
{
    public class CreateSkillRequest
    {
        [Required]
        [MaxLength(100)]
        public string SkillName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Category { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Recruitment.Requests
{
    public class CreateJobRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(100)]
        public string? ExperienceLevel { get; set; }

        [MaxLength(100)]
        public string? EducationLevel { get; set; }

        [Required]
        public int RecruiterId { get; set; }
    }
}

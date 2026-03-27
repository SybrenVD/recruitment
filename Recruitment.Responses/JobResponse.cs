namespace Recruitment.Responses
{
    public class JobResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? EducationLevel { get; set; }
        public int RecruiterId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

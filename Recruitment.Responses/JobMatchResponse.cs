namespace Recruitment.Responses
{
    public class JobMatchResponse
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public int MatchScore { get; set; }
        public string? SkillGap { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace Recruitment.Responses
{
    public class CandidateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? CVFilePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

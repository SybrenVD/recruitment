using System;
using System.Collections.Generic;

namespace Recruitment.Entities
{
    public class Recruiter
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Company { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}

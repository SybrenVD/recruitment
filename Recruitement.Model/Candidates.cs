using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitement.Model
{
    [Table(nameof(Candidates))]
    public class Candidates
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; } 
        public required string location { get; set; }
        public required string CVFilePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

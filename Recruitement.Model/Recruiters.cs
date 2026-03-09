using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitement.Model
{
    [Table(nameof(Recruiters))]
    public class Recruiters
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Company { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

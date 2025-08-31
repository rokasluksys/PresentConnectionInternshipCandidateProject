using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Contact
    {
        [Key]
        public int ContactID { get; set; }
        public required string Address { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }

        public ICollection<Package> SentPackages { get; set; } = new List<Package>();
        public ICollection<Package> ReceivedPackages { get; set; } = new List<Package>();

    }
}

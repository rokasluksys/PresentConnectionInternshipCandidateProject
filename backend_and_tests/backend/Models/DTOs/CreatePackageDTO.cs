using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs
{
    public class CreatePackageDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public CreateContactDTO Sender { get; set; } = null!;

        [Required]
        public CreateContactDTO Recipient { get; set; } = null!;
    }

    public class CreateContactDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;
    }
}

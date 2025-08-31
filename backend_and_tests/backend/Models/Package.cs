using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Package
    {
        [Key]
        public int TrackingNumber { get; set; }
        public int SenderID { get; set; }
        public int RecipientID { get; set; }
        //public int StatusID { get; set; }

        public required string Name { get; set; }

        public required Contact Sender { get; set; }
        public required Contact Recipient { get; set; }
        public ICollection<Status> Statuses { get; set; } = new List<Status>();

    }
}

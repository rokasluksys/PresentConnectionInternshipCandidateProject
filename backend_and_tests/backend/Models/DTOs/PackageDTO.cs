namespace backend.Models.DTOs
{
    public class PackageDTO
    {
        public int TrackingNumber { get; set; }
        public string Name { get; set; } = string.Empty;

        public ContactDTO Sender { get; set; } = null!;
        public ContactDTO Recipient { get; set; } = null!;
        public List<StatusDTO> Statuses { get; set; } = new();
    }

    public class ContactDTO
    {
        public int ContactID { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
    }

    public class StatusDTO
    {
        public string StatusName { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }

}

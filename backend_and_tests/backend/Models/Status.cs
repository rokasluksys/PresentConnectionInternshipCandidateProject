using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Status
    {
        [Key]
        public int StatusID { get; set; }
        public int PackageTrackingNumber { get; set; }
        public StatusTypes StatusName { get; set; }
        public DateTime Timestamp { get; set; }

        public Package Package { get; set; } = null!;

        public enum StatusTypes { Created, Sent, Returned, Accepted, Canceled }
    }
}

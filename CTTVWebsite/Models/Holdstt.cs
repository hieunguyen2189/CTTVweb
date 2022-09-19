using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class Holdstt
    {
        public string? SERIAL_NUMBER { get; set; }
        public string? MODEL_NAME { get; set; }
        public string? HOLD_ID { get; set; }
        public string? HOLD_REASON { get; set; }
        public string? STOP_GROUP { get; set; }
    }
}
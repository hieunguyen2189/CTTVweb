using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class OnlineOver
    {
        public string LINE_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string MO_NUMBER { get; set; }
        public string GROUP_NAME { get; set; }
        public DateTime IN_STATION_TIME { get; set; }
    }
}

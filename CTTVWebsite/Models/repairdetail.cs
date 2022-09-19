using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class repairdetail
    {
        public string SERIAL_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string LINE_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string IN_STATION_TIME { get; set; }
        public string NEXT_STATION { get; set; }
    }
}

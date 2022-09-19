using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class rpinfo
    {
        public string MODEL_NAME { get; set; }
        public string LINE_NAME { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string TEST_TIME { get; set; }
        public string TEST_STATION { get; set; }
        public string REPAIR_TIME { get; set; }
        public string ITEM_DESC { get; set; }
        public string MEMO { get; set; }
        public string GROUP_NAME { get; set; }
        public string IN_STATION_TIME { get; set; }
    }
}

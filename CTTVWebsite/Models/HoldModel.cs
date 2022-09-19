using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class HoldModel
    {
        public string SERIAL_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string HOLD_NO { get; set; }
        public string APPLY_REASON { get; set; }
        public string GROUP_NAME { get; set; }
        public DateTime IN_STATION_TIME { get; set; }
        public string PALLET_NO { get; set; }

    }
}

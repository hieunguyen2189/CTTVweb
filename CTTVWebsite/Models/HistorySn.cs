using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models

{
    [Keyless]
    public class HistorySn
    {
        public string? SERIAL_NUMBER { get; set; }
        public string? MODEL_NAME { get; set; }
        public string? OLD_GROUP_NAME { get; set; }
        public string? NOW_GROUP_NAME { get; set; }
        public string? PALLET_NO { get; set; }
        public int? NUM_PASS { get; set; }
        public DateTime? IN_STATION_TIME { get; set; }

        public DateTime? LAST_IN_STATION_TIME { get; set; }
    }
}
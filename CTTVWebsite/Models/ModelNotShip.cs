using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class ModelNotShip
    {
        public string SERIAL_NUMBER { get; set;}
        public string GROUP_NAME_NOW { get; set;}
        public string MODEL_NAME { get; set;}
        public string PALLET_NO { get; set;}
        public string? LAST_PACKING4_TIME { get; set;}
        public string? LAST_IN_STATION_TIME { get; set;}
        public string? QA_JUDGE { get; set; }
        public string? HOLD_STATUS { get; set; }
        public string? MO_NUMBER { get; set; }
        public string LINE_NAME { get; set; }
    }
}

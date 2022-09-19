using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class OBI_EMP
    {
        public string SERIAL_NUMBER { get; set; }
        public string EMP_NO { get; set; }
        public string GROUP_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public DateTime IN_STATION_TIME { get; set; }
        public string? PALLET_NO { get; set; }
    }
}

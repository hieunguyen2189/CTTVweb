using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class PONO
    {
        public string MODEL_NAME { get; set; }
        public string BOM_NO { get; set; }
        public string PALLET_NO { get; set;}
        public string SERIAL_NUMBER { get; set;}
        public string PO_NO { get; set; }
        public string GROUP_NAME { get; set; }
        public string CONTAINER_NO { get; set; }
        public DateTime IN_STATION_TIME { get; set; }
        public string KEY_PART_NO { get; set; }


    }
}

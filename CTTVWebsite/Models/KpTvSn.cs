using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class KpTvSn
    {
        public string SERIAL_NUMBER { get; set; }
        public string? KEY_PART_NO { get; set; }
        public string KEY_PART_SN { get; set; }
        public string GROUP_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public DateTime WORK_TIME { get; set; }
        public string OBI_TIME { get; set; }
        public string QA_TIME { get; set; }
        public DateTime IN_STATION_TIME { get; set; }
        public string PALLET_NO { get; set; }
        public string CONTAINER_NO { get; set; }
        public string OBI_RECORD { get; set; }
        public string QA_RECORD { get; set; }
    }
}

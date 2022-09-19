using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class Wiptracking
    {
        public string? SERIAL_NUMBER { get; set; }
        public string? MODEL_NAME { get; set; }
        public string? GROUP_NAME { get; set; }
        public string? LINE_NAME { get; set; }
        public string? PALLET_NO { get; set; }
        public DateTime? IN_STATION_TIME { get; set; }
        //public bool? SECTION_FLAG { get; set; }
        public string? MO_NUMBER { get; set; }
        public string? GROUP_NEXT { get; set; }
        public string CONTAINER_NO { get; set; }
        public string PO_NO { get; set; }
        public string OBI_RECORD { get; set; }
        public string OBI_IN_TIME { get; set; }
        public string CHECK_BOX { get; set; }
        public string OBI_BOX_TIME { get; set; }
        public string CHECK_SN { get; set; }
        public string OBI_SN_TIME { get; set; }
        //public string? NEXT_STATION { get; set; }
    }
    
}

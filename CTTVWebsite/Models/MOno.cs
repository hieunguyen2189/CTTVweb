using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class MOno
    {
        public string? SERIAL_NUMBER { get; set; }
        public string? MODEL_NAME { get; set; }
        public string? GROUP_NAME { get; set; }
        public string? PALLET_NO { get; set; }
        //public bool? SECTION_FLAG { get; set; }
        public string? MO_NUMBER { get; set; }
        public string? PACKING4 { get; set; }
        public string? INPUT { get; set; }
    }
}

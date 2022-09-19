using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class ApplyTable
    {
        public string? TABLE_NO { get; set; }
        public string APPLY_REASON { get; set;}
        public string TOTAL_COUNT { get; set;}
        public DateTime APPLY_TIME { get; set;}
        public int? HOLD_DAYS { get; set;}
        public int? HOLD_NO { get; set; }
        public int? UNHOLD_NO { get; set; }
    }
}

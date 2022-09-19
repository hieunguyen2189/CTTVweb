using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class repairsum
    {
        public string MODEL_NAME { get; set; }
        public string LINE_NAME { get; set; }
        public string SCAN_ERROR { get; set; }
        public string REPAIR_IN { get; set; }
        public string REPAIR_OUT { get; set; }
    }
}

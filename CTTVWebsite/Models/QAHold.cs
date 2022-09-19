using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class QAHold
    {
        public string? SERIAL_NUMBER { get; set; }
        public string? MODEL_NAME { get; set; }
        public string? GROUP_NAME { get; set; }
        public string? PALLET_NO { get; set; }
        public string? QA_JUDGE { get; set; }
        public string? HOLD_STATUS { get; set; }
    }
}

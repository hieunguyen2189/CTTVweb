using Microsoft.EntityFrameworkCore;
namespace CTTVWebsite.Models
{
    [Keyless]
    public class Bincode
    {
        public string? PALLET_NO { get; set; }
        public string? MODEL_NAME { get; set; }
        public string? WH_CODE { get; set; }
        public string? BIN_CODE { get; set; }
        public DateTime? WORK_TIME { get; set; }
        public string? QTY_PALLET { get; set; }
    }
}

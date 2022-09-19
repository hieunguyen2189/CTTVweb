using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class Stock
    {
        public string? BIN_CODE { get; set; }
        public string? WAREHOUSE_CODE { get; set; }
        public string? MODEL_TYPE { get; set; }
        public string? MAX_PALLET_QTY { get; set; }
        public string? NUM { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class StockInven
    {
        public string? MODEL_NAME { get; set; }
        public string? CTTV_WH_SN_QTY { get; set; }
        public string? CTTV_WH_PALLET_QTY { get; set; }
        public string? CTTVOUT_WH_SN_QTY { get; set; }
        public string? CTTVOUT_PALLET_QTY { get; set; }
        public string? DEEPC_WH_SN_QTY { get; set; }
        public string? DEEPC_WH_PALLET_QTY { get; set; }
    }
}

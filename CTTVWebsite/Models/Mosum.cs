using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class Mosum
    {
        public string MO_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string TARGET_QTY { get; set; }
        public string INPUT_QTY { get; set; }
        public string OUTPUT_QTY { get; set; }
        public string SHIPPING_QTY { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class OutSum
    {
        public string MODEL_NAME { get; set; }
        public string CTTV_OK { get; set; }
        public string CTTV_NOK { get; set; }
        public string CTTV_TOTAL { get; set; }
        public string DEEPC_OK { get; set; }
        public string DEEPC_NOK { get; set; }
        public string DEEPC_TOTAL { get; set; }
        public string CTTVOUT_OK { get; set; }
        public string CTTVOUT_NOK { get; set; }

        public string CTTVOUT_TOTAL { get; set; }
    }
}

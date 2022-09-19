using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class FacSum
    {
        public string MODEL_NAME { get; set; }
        public string TOTAL_PLAN { get; set;  }
        public string NOT_PRODUCE { get; set; }
        public string IN_FACTORY { get; set; }
        public string SHIPPING { get; set; }

    }
}

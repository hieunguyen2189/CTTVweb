using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class Class
    {
        public string PLAN_DATE { get; set; }
        public string LINE_NAME  { get; set; }
        public string MODEL_NAME { get; set; }
        public string SHIFT  { get; set; }
        public string PLAN_QTY { get; set; }
        public string OUTPUT { get; set; }
        public string COMPLETED_PERCENT { get; set; }
        public string REPAIR { get; set; }
        public string BOM_NO { get; set; }
    }
}

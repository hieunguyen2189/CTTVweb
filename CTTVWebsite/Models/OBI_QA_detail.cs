using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class OBI_QA_detail
    {
        public string MODEL_NAME { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public DateTime FIRST_DATE_TIME { get; set; }
        public DateTime LAST_DATE_TIME {get; set; }
        public int PASSED_TIME { get; set; }
        public string EMP_NO{ get; set; }
    }
}

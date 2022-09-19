using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class OBI
    {
        public string MODEL_NAME { get; set; }
        public int OBI_IN { get; set; }
        public int LCM_QA_CHKBOX { get; set; }

        public int LCM_QA_CHKSN { get; set; }
        public int OBI_TV_CHK1 { get; set; }
        public int OBI_TV_OUT { get; set; }
        public int AGIN_ASSY { get; set; }
        public int AGOUT_ASSY { get; set; }
        public int OBI_INT_CHK { get; set; }
        public int OBI_OUT { get; set; }
    }
}

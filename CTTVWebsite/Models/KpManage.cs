using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class KpManage
    {
        public string KEY_PART_NO { get; set; }
        public string CHECK_RESULT { get; set; }
        //public string KP_LENGTH { get; set;  }
        //public string STR_START1 { get; set; }
        //public string STR_END1 { get; set; }
        //public string COM_STR1 { get; set; }
        //public string STR_START2 { get; set; }
        //public string STR_END2 { get; set; }
        //public string COM_STR2 { get; set; }

    }
}

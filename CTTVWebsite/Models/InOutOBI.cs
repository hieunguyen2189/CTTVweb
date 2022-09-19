using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class InOutOBI
    {
       
        public string MODEL_NAME { get; set; }
        public string OBI_IN { get; set;}
        public string OBI_OUT { get; set;}
        public string SHIFT { get; set;}

    }
}

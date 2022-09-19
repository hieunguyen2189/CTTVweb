using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class InOutTotal
    {
        public string MODEL_NAME { get; set; }

        public string INPUT     { get; set;}
        public string OUTPUT { get; set;}
    }
}

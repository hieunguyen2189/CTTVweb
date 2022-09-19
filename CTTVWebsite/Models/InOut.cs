using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class InOut
    {
        public string MODEL_NAME { get; set; }
        public string LINE_NAME { get; set;}
        public string INPUT     { get; set;}
        public string OUTPUT { get; set;}
        public string SHIFT { get; set;}
    }
}

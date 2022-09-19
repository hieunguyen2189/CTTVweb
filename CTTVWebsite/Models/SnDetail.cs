using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class SnDetail
    {
        public string? SERIAL_NUMBER { get; set; }
        public string? GROUP_NAME { get; set; }
        public string? MODEL_NAME { get; set; }
        [NotMapped]
        public DateTime? IN_STATION_TIME { get; set; }
        public string? LINE_NAME { get; set; }
        public DateTime? FIRST_TIME { get; set; }
        public DateTime? LAST_TIME { get; set; }
        public String? NUM { get; set; }

    }
}

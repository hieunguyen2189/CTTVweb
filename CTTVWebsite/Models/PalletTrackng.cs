using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class PalletTrackng { 
        public string? SERIAL_NUMBER { get; set; }
    public string? MODEL_NAME { get; set; }
    public string? GROUP_NAME { get; set; }
    public string? PALLET_NO { get; set; }
    public DateTime? IN_STATION_TIME { get; set; }
}
}

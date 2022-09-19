using System.ComponentModel.DataAnnotations;

namespace CTTVWebsite.Models
{
    public class OldKP
    {
        [Key]
        public string SERIAL_NO { get; set; }
        [Required]
        public String OLD_KP_SN { get; set; }
        public String OLD_KP_NO { get; set; }
        public DateTime CHANGE_TIME { get; set; }
    }
}

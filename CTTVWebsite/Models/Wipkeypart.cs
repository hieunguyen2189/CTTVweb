using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTTVWebsite.Models
{
    public class Wipkeypart
    {
        [Key]
        public string SERIAL_NUMBER { get; set; }
        [Required]
          public String  KEY_PART_NO { get; set; }
          public String  KEY_PART_SN { get; set; }
           public String  GROUP_NAME { get; set; }
        public String MODEL_NAME { get; set; }
        public DateTime WORK_TIME { get; set; }
    }
}

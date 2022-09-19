using Microsoft.EntityFrameworkCore;

namespace CTTVWebsite.Models
{
    [Keyless]
    public class InstockSearch
    {
       public string  SERIAL_NUMBER { get; set; }
      public string  MODEL_NAME { get; set; }
     public string  GROUP_NAME { get; set; }
      public DateTime INSTOCK_TIME { get; set; }
      public string  PALLET_NO { get; set; }
     public string WH_CODE { get; set; }
     public string BIN_CODE { get; set; }
        public string QA_JUDGE { get; set; }
        public string HOLD_STATUS { get; set; }

    }
}

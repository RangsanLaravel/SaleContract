using System;
using System.Collections.Generic;
using System.Text;

namespace SaleContractAPI.DataContract
{
    public class tbt_sale_notification
    {
        public string ID { get; set; }
          public string companyid{ get; set; }
          public DateTime? noti_dt{ get; set; }
          public string Followup_dt{ get; set; }
          public string accept_dt{ get; set; }
          public string create_dt{ get; set; }
    }
}

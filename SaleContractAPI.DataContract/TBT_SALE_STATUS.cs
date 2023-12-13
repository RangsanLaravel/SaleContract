using System;
using System.Collections.Generic;
using System.Text;

namespace SaleContractAPI.DataContract
{
    public class TBT_SALE_STATUS
    {
        public int ID { get; set; }
        public int company_id { get; set; }
        public string status_code { get; set; }
        public string status_description { get; set; }
        public string fsystem_id { get; set; }
        public DateTime? fsystem_dt { get; set; }
        public string tmn_flg { get; set; }
        public DateTime? tmn_dt { get; set; }
        public string remark { get; set; }
        
    }
}

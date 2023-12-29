using System;
using System.Collections.Generic;
using System.Text;

namespace SaleContractAPI.DataContract
{
    public class TBT_SALE_STATUS 
    {
        public int ID { get; set; }
        public int company_id { get; set; }
        public string name { get; set; }
        public string website { get; set; }
        public string contract { get; set; }
        public string position { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string modelType { get; set; }
        public string Location { get; set; }
        public string People { get; set; }
        public string persen { get; set; }
        public string Dealvalue { get; set; }
        public string Dealcreationdate { get; set; }
        public string Duedatefollowup { get; set; }
        public string status_code { get; set; }
        public string status_description { get; set; }
        public string fsystem_id { get; set; }
        public DateTime? fsystem_dt { get; set; }
        public string tmn_flg { get; set; }
        public DateTime? tmn_dt { get; set; }
        public string remark { get; set; }
        public string priority { get; set; }
        public string noti_dt { get; set; }
        public DateTime? due_dt { get; set; }
        public List<tbt_remark_status> remark_Statuses { get; set; }
    }
}

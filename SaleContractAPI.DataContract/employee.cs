using System;
using System.Collections.Generic;
using System.Text;

namespace SaleContractAPI.DataContract
{
    public class employee
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
        public object password { get; set; }
        public string fullname { get; set; }
        public string lastname { get; set; }
        public string idcard { get; set; }
        public string position { get; set; }
        public string position_description { get; set; }
        public string showstock { get; set; }
        public string location_name { get; set; }
        public string location_id { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public string create_date { get; set; }
        public string create_by { get; set; }
        public string update_date { get; set; }
        public string update_by { get; set; }
    }
}

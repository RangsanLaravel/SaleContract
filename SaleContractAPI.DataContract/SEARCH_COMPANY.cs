using System;
using System.Collections.Generic;
using System.Text;

namespace SaleContractAPI.DataContract
{
    public class SEARCH_COMPANY
    {
        public string ID { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string NAME { get; set; }
        public string Owner { get; set; }
        public string MOBILE { get; set; }
        public string EMAIL { get; set; }
        public string DealDateFollowup { get; set; }
    }
}

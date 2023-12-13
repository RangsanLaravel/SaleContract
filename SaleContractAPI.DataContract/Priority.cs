using System;
using System.Collections.Generic;
using System.Text;

namespace SaleContractAPI.DataContract
{
    public class Priority
    {
        public string ID { get; set; }
        public string Priority_SEQ { get; set; }
        public string Priority_description { get; set; }
        public string TMN_FLG { get; set; }
    }
}

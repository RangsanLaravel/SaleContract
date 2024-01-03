using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleContractAPI.DataContract
{
    public class email_service
    {
        public string email { get; set; }
        public List<string> cc { get; set; }
        public string bcc { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string userid { get; set; }
    }
}

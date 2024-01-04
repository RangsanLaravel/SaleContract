using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SaleContractAPI.DataContract
{
    public class company_detail
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string WEBSITE { get; set; }
        public string Contract { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Owner { get; set; }
        public string ModelType { get; set; }
        public string People { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Priority_description { get; set; }
        public DateTime? DealCreate { get; set; }
        public DateTime? DealDateFollowup { get; set; }
        public DateTime? DealDateNoti { get; set; }
        public string DealValue { get; set; }
        [DisplayName("Percen")]
        public string Persen { get; set; }
        public string location { get; set; }
        public string Won { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}

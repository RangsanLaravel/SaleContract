using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SaleContractAPI.BusinessLogic
{
    public class ServiceAction
    {
        private readonly string _connectionstring = string.Empty;
        private readonly CultureInfo culture = new CultureInfo("th-TH");
        private readonly string DBENV = string.Empty;
        public ServiceAction(string connectionstring, string DBENV)
        {
            this._connectionstring = connectionstring;
            this.DBENV = DBENV;
        }
    }
}

using SaleContractAPI.DataContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace SaleContractAPI.DataAccess
{
    public partial class Repository
    {
        #region "PROCEDURE"
        public async ValueTask<DataTable> SP_GET_REPORT_CRM_BY_SALE()
        {
            SqlCommand cmd = new SqlCommand($"[{DBENV}].[dbo].[SP_GET_REPORT_CRM_BY_SALE]", this.sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = this.transaction;
            
            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(cmd))
            {
                if (dt.Rows.Count > 0)
                {
                    dt.TableName = "SP_GET_REPORT_CRM_BY_SALE";
                    return dt ;
                }
                else
                    return null;
            }

        }

        public async ValueTask<DataTable> SP_GET_REPORT_CRM_BY_STATUS_SALE()
        {
            SqlCommand cmd = new SqlCommand($"[{DBENV}].[dbo].[SP_GET_REPORT_CRM_BY_STATUS_SALE]", this.sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = this.transaction;
            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(cmd))
            {
                if (dt.Rows.Count > 0)
                {
                    dt.TableName = "SP_GET_REPORT_CRM_BY_STATUS_SALE";
                    return dt;
                }
                else
                    return null;
            }

        }
        #endregion "PROCEDURE"
    }
}

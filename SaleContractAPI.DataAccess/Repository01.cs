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
        public async ValueTask<DataTable> SP_GET_REPORT_CRM_BY_SALE(string userid)
        {
            SqlCommand cmd = new SqlCommand($"[{DBENV}].[dbo].[SP_GET_REPORT_CRM_BY_SALE]", this.sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = this.transaction;
            if (!string.IsNullOrWhiteSpace(userid))
            {
                cmd.Parameters.AddWithValue("@id", userid);
            }
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

        public async ValueTask<DataTable> SP_GET_REPORT_CRM_BY_STATUS_SALE(string userid,SEARCH_COMPANY condition)
        {
            SqlCommand cmd = new SqlCommand($"[{DBENV}].[dbo].[SP_GET_REPORT_CRM_BY_STATUS_SALE]", this.sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = this.transaction;
            if (!string.IsNullOrWhiteSpace(userid))
            {
                cmd.Parameters.AddWithValue("@id", userid);           
            }
            cmd.Parameters.AddWithValue("@priority", condition?.Priority?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", condition?.Status?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", condition?.NAME?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@modeltype", condition?.ModelType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@contract", condition?.Contract ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@lastupdate", condition?.lastupdate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ownername", condition?.ownername ?? (object)DBNull.Value);

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

        public async ValueTask<DataTable> SP_GET_REPORT_CRM_BY_SERVICE(string userid)
        {
            SqlCommand cmd = new SqlCommand($"[{DBENV}].[dbo].[SP_GET_REPORT_CRM_BY_SERVICE]", this.sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = this.transaction;
            if (!string.IsNullOrWhiteSpace(userid))
            {
                cmd.Parameters.AddWithValue("@id", userid);
            }
            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(cmd))
            {
                if (dt.Rows.Count > 0)
                {
                    dt.TableName = "SP_GET_REPORT_CRM_BY_SERVICE";
                    return dt;
                }
                else
                    return null;
            }

        }
        #endregion "PROCEDURE"

        #region "Terminate Data"
        public async ValueTask TERMINATE_TBT_COMPANY_DETAIL(string id)
        {
            SqlCommand cmd = new SqlCommand($"UPDATE [{DBENV}].[dbo].[tbt_company_detail] SET tmn_dt =GETDATE(),tmn_flg='Y' WHERE ID =@ID", this.sqlConnection);
            cmd.CommandType = CommandType.Text;
            cmd.Transaction = this.transaction;
            cmd.Parameters.Add(new SqlParameter("ID", id));
            await  cmd.ExecuteNonQueryAsync();
        }
        public async ValueTask TERMINATE_TBT_COMPANY_DETAIL_JOB(string id)
        {
            SqlCommand cmd = new SqlCommand($"UPDATE [{DBENV}].[dbo].[tbt_company_detail_JOB] SET tmn_dt =GETDATE(),tmn_flg='Y' WHERE ID =@ID", this.sqlConnection);
            cmd.CommandType = CommandType.Text;
            cmd.Transaction = this.transaction;
            cmd.Parameters.Add(new SqlParameter("ID", id));
            await cmd.ExecuteNonQueryAsync();
        }
        #endregion "Terminate Data"
    }
}

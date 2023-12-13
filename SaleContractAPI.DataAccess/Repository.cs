using System.Data.SqlClient;
//using Microsoft.IdentityModel.Tokens;
using SaleContractAPI.DataContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using ITUtility;
using DataAccessUtility;
using System.Linq;

namespace SaleContractAPI.DataAccess
{
    public class Repository
    {
        #region " STATIC "

        private readonly SqlConnection sqlConnection = null;
        private SqlTransaction transaction;

        private readonly string DBENV = string.Empty;
        public Repository(string connectionstring, string DBENV) : this(new SqlConnection(connectionstring), DBENV)
        {

        }
        public Repository(SqlConnection ConnectionString, string DBENV)
        {
            this.sqlConnection = ConnectionString;
            this.DBENV = DBENV;
        }
        public async ValueTask OpenConnectionAsync() =>
       await this.sqlConnection.OpenAsync();
        public async ValueTask CloseConnectionAsync() =>
       await this.sqlConnection.CloseAsync();

        public async ValueTask beginTransection() =>
            this.transaction = this.sqlConnection.BeginTransaction();

        public async ValueTask CommitTransection() =>
            this.transaction.Commit();


        public async ValueTask RollbackTransection() =>
            this.transaction.Rollback();
        #endregion " STATIC "

        #region " GET "
        public async ValueTask<List<substatus>> GET_STATUS(string status_type)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = $@"SELECT [id]
      ,[STATUS_CODE]
      ,[STATUS_DESCRIPTION]
      ,[STATUS_SEQ]
      ,[ACTIVE_FLG]
      ,[STATUS_TYPE]
  FROM [{DBENV}].dbo.[tbm_substatus]
  WHERE ACTIVE_FLG =1
  AND STATUS_TYPE = @STATUS_TYPE
  ORDER BY STATUS_SEQ ASC"
            };
            sql.Parameters.AddWithValue("@STATUS_TYPE", status_type);

            using (DataTable dt = await Utility.FillDataTableAsync(sql))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<substatus>().ToList();
                }
                else
                {
                    return null;
                }
            }

        }

        public async ValueTask<List<Priority>> GET_PRIORITY()
        {
            SqlCommand command = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = $@"SELECT  [ID]
                                    ,[Priority_description]
                                    ,[Priority_SEQ]
                                    ,[TMN_FLG]
            FROM [{DBENV}].dbo.[tbm_priority]
            WHERE TMN_FLG ='N'"
            };
            
            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(command))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<Priority>().ToList();
                }
                else
                    return null;
            }
        }


        public async ValueTask<List<TBT_SALE_STATUS>> GET_TBT_SALE_STATUS(int company_id)
        {
            SqlCommand command = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = $@"SELECT tbtst.ID
      ,tbtst.company_id
      ,tbtst.status_code
	  ,sub.STATUS_DESCRIPTION
      ,tbtst.fsystem_id
      ,tbtst.fsystem_dt
      ,tbtst.tmn_flg
      ,tbtst.tmn_dt
      ,tbtst.remark
  FROM [{DBENV}].dbo.[tbt_sale_status] tbtst
  INNER JOIN [{DBENV}].dbo.[tbm_substatus] sub on sub.STATUS_CODE= tbtst.STATUS_CODE
  WHERE tbtst.tmn_flg='N'
  AND sub.ACTIVE_FLG =1
  AND sub.STATUS_TYPE='SALE'
  AND tbtst.company_id =@company_id 
ORDER BY tbtstใFSYSTEM_DT ASC"
            };
            command.Parameters.AddWithValue("@company_id", company_id);
            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(command))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<TBT_SALE_STATUS>().ToList();
                }
                else
                    return null;
            }
        }

        public async ValueTask<List<company_detail>> GET_COMPANY(SEARCH_COMPANY condition)
        {
            SqlCommand command = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = $@"SELECT [ID]
      ,[NAME]
      ,[WEBSITE]
      ,[Contract]
      ,[Position]
      ,[Email]
      ,[Mobile]
      ,[Owner]
      ,[ModelType]
      ,[People]
      ,[Status]
      ,[Priority]
      ,[DealCreate]
      ,[DealDateFollowup]
      ,[DealDateNoti]
      ,[DealValue]
      ,[Won]
      ,[LastUpdate]
      ,[tmn_flg]
      ,[tmn_dt]
	  ,(SELECT Priority_description FROM [{DBENV}].dbo.[tbm_priority] WHERE TMN_FLG ='N' AND ID= cmp.Priority) as Priority_description 
  FROM [{DBENV}].dbo.[tbt_company_detail] cmp
  WHERE TMN_FLG ='N'
  AND (OWNER IS NULL OR Owner =@wner)
  AND  (ID IS NULL OR ID =@ID)
  AND (MOBILE IS NULL OR MOBILE =@MOBILE)
  AND (EMAIL IS NULL OR EMAIL =@EMAIL)
  AND (Priority IS NULL OR PRIORITY =@Priority)
  AND (Status IS NULL OR Status =@Status)
  AND (DealDateFollowup IS NULL OR DealDateFollowup=@DealDateFollowup)"
            };
            command.Parameters.AddWithValue("@wner", condition.Owner ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ID", condition.ID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MOBILE", condition.MOBILE ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EMAIL", condition.EMAIL ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Priority", condition.Priority ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Status", condition.Status ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DealDateFollowup", condition.DealDateFollowup ?? (object)DBNull.Value);

            using (DataTable dt = await Utility.FillDataTableAsync(command))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<company_detail>().ToList();
                }
                else
                    return null;
            }
        }

        #endregion " GET "

        #region " INSERT "

        public async ValueTask INSERT_TBT_COMPANY_DETAIL(company_detail condition)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                Transaction = this.transaction,
                CommandText = $@"INSERT INTO [{DBENV}].[dbo].[tbt_company_detail]
           ([NAME]
           ,[WEBSITE]
           ,[Contract]
           ,[Position]
           ,[Email]
           ,[Mobile]
           ,[Owner]
           ,[ModelType]
           ,[People]
           ,[Priority]
           ,[DealCreate]
           ,[DealDateFollowup]
           ,[DealDateNoti]
           ,[DealValue]
           ,[Won]
           ,[LastUpdate]
           ,[tmn_flg]
           ,[tmn_dt])
     VALUES
           (@NAME
           ,@WEBSITE
           ,@Contract
           ,@Position
           ,@Email
           ,@Mobile
           ,@Owner
           ,@ModelType
           ,@People
           ,@Priority
           ,GETDATE()
           ,@DealDateFollowup
           ,@DealDateNoti
           ,@DealValue
           ,@Won
           ,GETDATE()
           ,'N'
           ,NULL)"
            };

            sql.Parameters.AddWithValue("@NAME", condition.NAME ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@WEBSITE", condition.WEBSITE ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@Contract", condition.Contract ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@Position", condition.Position ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@Email", condition.Email ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@Mobile", condition.Mobile ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@Owner", condition.Owner ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@ModelType", condition.ModelType ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@People", condition.People ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@Priority", condition.Priority ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@DealDateFollowup", condition.DealDateFollowup ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@DealDateNoti", condition.DealDateNoti ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@DealValue", condition.DealValue ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@Won", condition.Won ?? (object)DBNull.Value);

            await sql.ExecuteNonQueryAsync();
        }

        public async ValueTask INSERT_TBT_SALE_STATUS(TBT_SALE_STATUS condition)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                Transaction = this.transaction,
                CommandText = $@"INSERT INTO [{DBENV}].[dbo].[tbt_sale_status]
           ([company_id]
           ,[status_code]
           ,[fsystem_id]
           ,[fsystem_dt]
           ,[tmn_flg]
           ,[tmn_dt]
           ,[remark])
     VALUES
           (@company_id
           ,@status_code
           ,@fsystem_id
           ,GETDATE()
           ,'N'
           ,NULL
           ,@remark)"
            };

            sql.Parameters.AddWithValue("@company_id", condition.company_id );
            sql.Parameters.AddWithValue("@status_code", condition.status_code ??(object)DBNull.Value );
            sql.Parameters.AddWithValue("@fsystem_id", condition.fsystem_id );
            sql.Parameters.AddWithValue("@remark", condition.remark ?? (object)DBNull.Value);


            await sql.ExecuteNonQueryAsync();
        }

        #endregion " INSERT "

        #region " UPDATE "
        public async ValueTask UPDATE_TBT_COMPANY_DETAIL(company_detail condition)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                Transaction = this.transaction,
                CommandText = $@"UPDATE [{DBENV}].[dbo].[tbt_company_detail]
SET Priority =@priority
WHERE ID =@ID "
            };

            sql.Parameters.AddWithValue("@priority", condition.Priority );
            sql.Parameters.AddWithValue("@ID", condition.ID );

            await sql.ExecuteNonQueryAsync();
        }

        #endregion " UPDATE "

    }
}

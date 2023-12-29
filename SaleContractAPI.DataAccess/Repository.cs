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
using Microsoft.IdentityModel.Tokens;

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

            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(sql))
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
            WHERE TMN_FLG ='N'
ORDER BY Priority_SEQ ASC"
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
      ,(select CONCAT(fullname,' ',lastname)  FROM [{DBENV}].[dbo].[tbm_employee] where user_id = tbtst.fsystem_id AND status =1) fsystem_id
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
ORDER BY tbtst.FSYSTEM_DT ASC"
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
                CommandText = $@"SELECT {(string.IsNullOrEmpty(condition.limit) ? "" : $"TOP {condition.limit}")}
            [ID]
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
      ,[location]
      ,[DealCreate]
      ,[DealDateFollowup]
      ,[DealDateNoti]
      ,[DealValue]
      ,Persen
      ,[Won]
      ,[LastUpdate]
      ,[tmn_flg]
      ,[tmn_dt]
	  ,(SELECT Priority_description FROM [{DBENV}].dbo.[tbm_priority] WHERE TMN_FLG ='N' AND ID= cmp.Priority) as Priority_description 
  FROM [{DBENV}].dbo.[tbt_company_detail] cmp
  WHERE TMN_FLG ='N'
  AND (@name IS NULL OR NAME like '%{condition.NAME}%')
  AND (@wner IS NULL OR Owner =@wner)
  AND  (@ID IS NULL OR ID =@ID)
  AND (@MOBILE IS NULL OR MOBILE =@MOBILE)
  AND (@EMAIL IS NULL OR EMAIL =@EMAIL)
  AND (@Priority IS NULL OR PRIORITY =@Priority)
  AND (@Status IS NULL OR Status =@Status)
  AND (@DealDateFollowup IS NULL OR DealDateFollowup=@DealDateFollowup)
   "
            };
            command.Parameters.AddWithValue("@name", string.IsNullOrWhiteSpace(condition.NAME) ? (object)DBNull.Value : condition.NAME);
            command.Parameters.AddWithValue("@wner", string.IsNullOrWhiteSpace(condition.Owner) ? (object)DBNull.Value: condition.Owner);
            command.Parameters.AddWithValue("@ID", string.IsNullOrWhiteSpace(condition.ID)? (object)DBNull.Value:condition.ID);
            command.Parameters.AddWithValue("@MOBILE", string.IsNullOrWhiteSpace(condition.MOBILE)? (object)DBNull.Value:condition.MOBILE);
            command.Parameters.AddWithValue("@EMAIL", string.IsNullOrWhiteSpace(condition.EMAIL)? (object)DBNull.Value:condition.EMAIL);
            command.Parameters.AddWithValue("@Priority", string.IsNullOrWhiteSpace(condition.Priority)? (object)DBNull.Value:condition.Priority);
            command.Parameters.AddWithValue("@Status", string.IsNullOrWhiteSpace(condition.Status)? (object)DBNull.Value:condition.Status);
            command.Parameters.AddWithValue("@DealDateFollowup", string.IsNullOrWhiteSpace(condition.DealDateFollowup)? (object)DBNull.Value:condition.DealDateFollowup);


            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(command))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<company_detail>().ToList();
                }
                else
                    return null;
            }
        }

        public async ValueTask<List<tbt_remark_status>> GET_tbt_remark_status(string IDSTATUS)
        {
            SqlCommand command = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = $@"SELECT 
        [ID]
      ,[ID_STATUS_SALE]
      ,[REMARK]
      ,[REMARK_DT]
      ,(select CONCAT(fullname,' ',lastname)  FROM [{DBENV}].[dbo].[tbm_employee] where user_id = REMARK_ID AND status =1) REMARK_ID
      ,[ID_REMARK_UPLINE]
      ,[TMN_FLG]     
FROM [{DBENV}].dbo.tbt_remark_status 
WHERE TMN_FLG ='N' 
AND ID_STATUS_SALE=@ID_STATUS_SALE
AND ID_REMARK_UPLINE IS NULL
ORDER BY ID_REMARK_UPLINE ASC"
            };
            command.Parameters.AddWithValue("@ID_STATUS_SALE", IDSTATUS);
            
            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(command))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<tbt_remark_status>().ToList();
                }
                else
                    return null;
            }
        }
        public async ValueTask<List<tbt_remark_status>> GET_tbt_remark_status_UPLINE(string IDREMARK)
        {
            SqlCommand command = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = $@"


WITH RecursiveCTE AS (
    -- Anchor member
    SELECT
        ID,
		ID_STATUS_SALE,
		REMARK,
		REMARK_DT,
		(select CONCAT(fullname,' ',lastname)  FROM [{DBENV}].[dbo].[tbm_employee] where user_id = REMARK_ID AND status =1) REMARK_ID,
		ID_REMARK_UPLINE
    FROM
        tbt_remark_status
    WHERE
        ID_STATUS_SALE IS NULL
		AND TMN_FLG ='N'
		AND ID_REMARK_UPLINE=@ID_REMARK_UPLINE

    UNION ALL

    -- Recursive member
    SELECT
		e.ID,
		e.ID_STATUS_SALE,
		e.REMARK,
		e.REMARK_DT,
		(select CONCAT(fullname,' ',lastname)  FROM [{DBENV}].[dbo].[tbm_employee] where user_id = e.REMARK_ID AND status =1) REMARK_ID,
		e.ID_REMARK_UPLINE
    FROM
        tbt_remark_status e
    INNER JOIN
        RecursiveCTE r ON e.ID_REMARK_UPLINE = r.ID
	WHERE  TMN_FLG ='N'
)
SELECT
    *
FROM
    RecursiveCTE
ORDER BY REMARK_DT ASC"
            };
            command.Parameters.AddWithValue("@ID_REMARK_UPLINE", IDREMARK);

            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(command))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<tbt_remark_status>().ToList();
                }
                else
                    return null;
            }
        }
        #endregion " GET "

        #region " INSERT "

        public async ValueTask<int> INSERT_TBT_COMPANY_DETAIL(company_detail condition)
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
           ,[location]
           ,[People]
           ,[Priority]
           ,[DealCreate]
           ,[DealDateFollowup]
           ,[DealDateNoti]
           ,[DealValue]          
           ,[Persen]
           ,[LastUpdate]
           ,[tmn_flg])
     VALUES
           (@NAME
           ,@WEBSITE
           ,@Contract
           ,@Position
           ,@Email
           ,@Mobile
           ,@Owner
           ,@ModelType
           ,@location
           ,@People
           ,@Priority
           ,GETDATE()
           ,@DealDateFollowup
           ,@DealDateNoti
           ,@DealValue
           ,@Persen
           ,GETDATE()
           ,'N')
;SELECT SCOPE_IDENTITY();"
            };

            sql.Parameters.AddWithValue("@NAME", condition.NAME ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@WEBSITE", string.IsNullOrEmpty(condition.WEBSITE )? (object)DBNull.Value:condition.WEBSITE);
            sql.Parameters.AddWithValue("@Contract", string.IsNullOrEmpty(condition.Contract) ? (object)DBNull.Value:condition.Contract);
            sql.Parameters.AddWithValue("@Position", string.IsNullOrEmpty(condition.Position) ? (object)DBNull.Value:condition.Position);
            sql.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(condition.Email) ? (object)DBNull.Value:condition.Email);
            sql.Parameters.AddWithValue("@Mobile", string.IsNullOrEmpty(condition.Mobile) ? (object)DBNull.Value:condition.Mobile);
            sql.Parameters.AddWithValue("@Owner", condition.Owner ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@ModelType", string.IsNullOrEmpty(condition.ModelType) ? (object)DBNull.Value:condition.ModelType);
            sql.Parameters.AddWithValue("@location",string.IsNullOrEmpty( condition.location) ? (object)DBNull.Value : condition.location);
            sql.Parameters.AddWithValue("@People",string.IsNullOrEmpty( condition.People)? (object)DBNull.Value :condition.People);
            sql.Parameters.AddWithValue("@Priority", string.IsNullOrEmpty(condition.Priority) ? (object)DBNull.Value:condition.Priority);
            sql.Parameters.AddWithValue("@DealDateFollowup", condition.DealDateFollowup ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@DealDateNoti", condition.DealDateNoti ?? (object)DBNull.Value);
            sql.Parameters.AddWithValue("@DealValue", string.IsNullOrEmpty(condition.DealValue) ? (object)DBNull.Value : condition.DealValue);
            sql.Parameters.AddWithValue("@Persen", string.IsNullOrEmpty(condition.Persen) ? (object)DBNull.Value:condition.Persen);

            var insertedId = await sql.ExecuteScalarAsync();

            // แปลงค่าที่รีเทิร์นมาเป็น int (หรือตามประเภทของ ID ของคุณ)
            return Convert.ToInt32(insertedId);
        }

        public async ValueTask<int> INSERT_TBT_SALE_STATUS(TBT_SALE_STATUS condition)
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
           ,@remark);SELECT SCOPE_IDENTITY();"
            };

            sql.Parameters.AddWithValue("@company_id", condition.company_id );
            sql.Parameters.AddWithValue("@status_code", condition.status_code ??(object)DBNull.Value );
            sql.Parameters.AddWithValue("@fsystem_id", condition.fsystem_id );
            sql.Parameters.AddWithValue("@remark", condition.remark ?? (object)DBNull.Value);


            var insertedId = await sql.ExecuteScalarAsync();

            // แปลงค่าที่รีเทิร์นมาเป็น int (หรือตามประเภทของ ID ของคุณ)
            return Convert.ToInt32(insertedId);
        }

        public async ValueTask INSERT_TBT_SALE_NOTIFICATION(tbt_sale_notification condition)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                Transaction = this.transaction,
                CommandText = $@"INSERT INTO [{DBENV}].[dbo].[tbt_sale_notification]
           (
           [company_id]
           ,[noti_dt]
           ,[create_dt])
     VALUES
           (
            @companyid
           ,@noti_dt
           ,GETDATE()
          )"
            };

            sql.Parameters.AddWithValue("@companyid", condition.companyid);
            sql.Parameters.AddWithValue("@noti_dt", condition.noti_dt);
            await sql.ExecuteNonQueryAsync();
        }

        public async ValueTask INSERT_TBT_REAMRK_STATUS(tbt_remark_status condition)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                Transaction = this.transaction,
                CommandText = $@"INSERT INTO [{DBENV}].[dbo].[tbt_remark_status]
           (
            [ID_STATUS_SALE]
           ,[REMARK]
           ,[REMARK_DT]
           ,[REMARK_ID]
           ,[TMN_FLG]
           ,[ID_REMARK_UPLINE])
     VALUES
           (
           @ID_STATUS_SALE
           ,@REMARK
           ,GETDATE()
           ,@REMARK_ID
           ,'N'
           ,@ID_REMARK_UPLINE)"
            };
            sql.Parameters.AddWithValue("@ID_STATUS_SALE", string.IsNullOrEmpty(condition.ID_STATUS_SALE) ? (object)DBNull.Value: condition.ID_STATUS_SALE);
            sql.Parameters.AddWithValue("@REMARK", string.IsNullOrEmpty(condition.REMARK) ? (object)DBNull.Value : condition.REMARK);
            sql.Parameters.AddWithValue("@REMARK_ID", string.IsNullOrEmpty(condition.REMARK_ID) ? (object)DBNull.Value : condition.REMARK_ID);
            sql.Parameters.AddWithValue("@ID_REMARK_UPLINE", string.IsNullOrEmpty(condition.ID_REMARK_UPLINE) ? (object)DBNull.Value : condition.ID_REMARK_UPLINE);

            await sql.ExecuteNonQueryAsync();
        }
        #endregion " INSERT "

        #region " UPDATE "
        public async ValueTask UPDATE_TBT_COMPANY(TBT_SALE_STATUS condition)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                Transaction = this.transaction,
                CommandText = $@"UPDATE [{DBENV}].[dbo].[tbt_company_detail]
SET Priority =@priority,
    Name=@name,
    website=@website,
    Contract=@Contract,
    Persen=@persen,
    Location=@location,
    Position=@Position,
    Email =@email,
    Mobile=@mobile,
    ModelType=@ModelType,
    People=@people,
    DealCreate=@DealCreate,
    DealDateFollowup=@DealDateFollowup,
    DealValue=@DealValue
WHERE ID =@ID "
            };

            sql.Parameters.AddWithValue("@priority",string.IsNullOrEmpty( condition.priority)? (object)DBNull.Value :condition.priority);
            sql.Parameters.AddWithValue("@website", string.IsNullOrEmpty( condition.website) ? (object)DBNull.Value :condition.website);
            sql.Parameters.AddWithValue("@name", string.IsNullOrEmpty( condition.name) ? (object)DBNull.Value :condition.name);
            sql.Parameters.AddWithValue("@Contract", string.IsNullOrEmpty( condition.contract)? (object)DBNull.Value :condition.contract);
            sql.Parameters.AddWithValue("@persen", string.IsNullOrEmpty( condition.persen)? (object)DBNull.Value :condition.priority);
            sql.Parameters.AddWithValue("@location", string.IsNullOrEmpty( condition.Location) ? (object)DBNull.Value :condition.Location);
            sql.Parameters.AddWithValue("@Position", string.IsNullOrEmpty( condition.position) ? (object)DBNull.Value :condition.position);
            sql.Parameters.AddWithValue("@email", string.IsNullOrEmpty( condition.email)? (object)DBNull.Value :condition.email);
            sql.Parameters.AddWithValue("@mobile", string.IsNullOrEmpty( condition.mobile)? (object)DBNull.Value :condition.mobile);
            sql.Parameters.AddWithValue("@ModelType", string.IsNullOrEmpty( condition.modelType)? (object)DBNull.Value :condition.modelType);
            sql.Parameters.AddWithValue("@people", string.IsNullOrEmpty( condition.People)? (object)DBNull.Value :condition.People);
            sql.Parameters.AddWithValue("@DealCreate", string.IsNullOrEmpty( condition.Dealcreationdate)? (object)DBNull.Value :condition.Dealcreationdate);
            sql.Parameters.AddWithValue("@DealDateFollowup", string.IsNullOrEmpty( condition.Duedatefollowup) ? (object)DBNull.Value :condition.Duedatefollowup);
            sql.Parameters.AddWithValue("@DealValue", string.IsNullOrEmpty( condition.Dealvalue) ? (object)DBNull.Value :condition.Dealvalue);
            sql.Parameters.AddWithValue("@ID", condition.company_id);

            await sql.ExecuteNonQueryAsync();
        }
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
        public async ValueTask UPDATE_TBT_COMPANY_DETAIL_STATUS(string STATUS,string ID)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                Transaction = this.transaction,
                CommandText = $@"UPDATE [{DBENV}].[dbo].[tbt_company_detail]
SET STATUS =@Status
WHERE ID =@ID "
            };

            sql.Parameters.AddWithValue("@Status", STATUS);
            sql.Parameters.AddWithValue("@ID", ID);

            await sql.ExecuteNonQueryAsync();
        }
        public async ValueTask UPDATE_TBT_COMPANY_DETAIL_WON(string ID)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                Transaction = this.transaction,
                CommandText = $@"UPDATE [{DBENV}].[dbo].[tbt_company_detail]
SET WON =GETDATE()
WHERE ID =@ID "
            };
 
            sql.Parameters.AddWithValue("@ID", ID);

            await sql.ExecuteNonQueryAsync();
        }

        public async ValueTask UPDATE_TBT_SALE_NOTIFICATION(string ID)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                Transaction = this.transaction,
                CommandText = $@"UPDATE [{DBENV}].[dbo].[tbt_sale_notification]
                                SET accept_dt =GETDATE()
                                WHERE ID =@ID "
            };
            sql.Parameters.AddWithValue("@ID", ID);

            await sql.ExecuteNonQueryAsync();
        }

      
        #endregion " UPDATE "

    }
}

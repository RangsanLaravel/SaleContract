using System.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
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

        public async ValueTask<List<substatus>> GET_STATUS(string fname)
        {
            SqlCommand sql = new SqlCommand
            {
                CommandType = System.Data.CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = $@"SELECT cus.customer_id,
		cus.cust_type,
		cus.address,
		(select td.district_name_tha from [{DBENV}].dbo.tbm_district  td where cus.district_code =td.district_code and td.status =1) as district_name_tha,
		(select std.sub_district_name_tha from [{DBENV}].dbo.tbm_sub_district std where std.sub_district_code = cus.sub_district_no and std.status=1) as sub_district_name_tha,
	    (select pr.province_name_tha from [{DBENV}].dbo.tbm_province pr where  pr.province_code =cus.province_code and pr.status =1) as province_name_tha,
		cus.zip_code,
		cus.phone_no,
		cus.Email,cus.fname
        FROM [{DBENV}].[dbo].[tbm_customer] cus
        WHERE fname LIKE @fname
              AND cus.status =1 "
            };
            sql.Parameters.AddWithValue("@fname", $"%{fname}%");

            using (DataTable dt = await ITUtility.Utility.FillDataTableAsync(sql))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<Customer>().ToList();
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
                CommandText = $@"select * from [{DBENV}].dbo.tbt_application_role rl
                    INNER JOIN [{DBENV}].dbo.tbm_application_center ap ON ap.application_id =rl.application_id and ap.application_status=1
                    WHERE rl.active_flg =1
                    AND  rl.user_id =@username"
            };
            command.Parameters.AddWithValue("@username", USERID);

            using (DataTable dt = await Utility.FillDataTableAsync(command))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<tbm_application_center>().ToList();
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
                CommandText = $@"select * from [{DBENV}].dbo.tbt_application_role rl
                    INNER JOIN [{DBENV}].dbo.tbm_application_center ap ON ap.application_id =rl.application_id and ap.application_status=1
                    WHERE rl.active_flg =1
                    AND  rl.user_id =@username"
            };
            command.Parameters.AddWithValue("@username", USERID);

            using (DataTable dt = await Utility.FillDataTableAsync(command))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable<tbm_application_center>().ToList();
                }
                else
                    return null;
            }
        }

        public async ValueTask GET_SALECONTRACT_STATUS()
        {

        }

        public async ValueTask InsertCompany(company_detail company_Detail)
        {

        }

        public async ValueTask UPDATE_STATUS_SALE()
        {

        }
        
    }
}

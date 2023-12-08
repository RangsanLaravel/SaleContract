using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ITUtility
{
    public static partial class Utility
    {
        public async static ValueTask<DataTable> FillDataTableAsync(SqlCommand oCmd)
        {
            using (SqlDataReader oReader = await oCmd.ExecuteReaderAsync())
            {
                using (DataTable dataTable = new DataTable())
                {
                    dataTable.Load(oReader);
                    oReader.Close();
                    return dataTable;
                }
            }
        }
        public static DataTable FillDataTable(string sqlStr, SqlConnection connection)
        {
            SqlDataAdapter oAdpt = new SqlDataAdapter(sqlStr, connection);
            DataTable dt = new DataTable();
            oAdpt.Fill(dt);
            oAdpt.Dispose();
            return dt;
        }
    }
}

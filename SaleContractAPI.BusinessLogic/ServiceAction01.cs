using OfficeOpenXml;
using SaleContractAPI.DataAccess;
using SaleContractAPI.DataContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace SaleContractAPI.BusinessLogic
{
    public partial class ServiceAction
    {

        public async ValueTask<byte[]> SP_GET_REPORT_CRM_BY_SALE(string userid)
        {
            byte[] excelfile =null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                var result =  await  repository.SP_GET_REPORT_CRM_BY_SALE(userid);
                if(result != null)
                {
                    excelfile = await DataTableToExcel(result);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await repository.CloseConnectionAsync();
            }
            return excelfile;
        }
        public async ValueTask<byte[]> SP_GET_REPORT_CRM_BY_STATUS_SALE(string userid)
        {
            byte[] excelfile = null;

            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                var result =  await  repository.SP_GET_REPORT_CRM_BY_STATUS_SALE(userid);
                if (result != null)
                {
                    excelfile = await DataTableToExcel(result);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await repository.CloseConnectionAsync();
            }
            return excelfile;

        }
        public async ValueTask<byte[]> SP_GET_REPORT_CRM_BY_SERVICE(string userid)
        {
            byte[] excelfile = null;

            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                var result = await repository.SP_GET_REPORT_CRM_BY_SERVICE(userid);
                if (result != null)
                {
                    excelfile = await DataTableToExcel(result);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await repository.CloseConnectionAsync();
            }
            return excelfile;

        }
        private async ValueTask<byte[]> DataTableToExcel(DataTable dataTable)
        {
            using (var package = new ExcelPackage())
            {
                
                // เพิ่มเวิร์กชีตใหม่
                var worksheet = package.Workbook.Worksheets.Add(dataTable.TableName);

                // เขียนชื่อคอลัมน์ไปยังเวิร์กชีต
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = dataTable.Columns[col].ColumnName;
                }

                // เขียนข้อมูลใน DataTable ไปยังเวิร์กชีต
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = dataTable.Rows[row][col];
                    }
                }

                // บันทึกลงใน MemoryStream แทนการบันทึกลงไฟล์
                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async ValueTask TERMINATE_TBT_COMPANY_DETAIL(string ID)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                await repository.TERMINATE_TBT_COMPANY_DETAIL(ID);
                await repository.CommitTransection();
            }
            catch (Exception ex)
            {
                await repository.RollbackTransection();
                throw ex;
            }
            finally
            {
                await repository.CloseConnectionAsync();
            }

        }

        public async ValueTask TERMINATE_TBT_COMPANY_DETAIL_JOB(string ID)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                await repository.TERMINATE_TBT_COMPANY_DETAIL_JOB(ID);
                await repository.CommitTransection();
            }
            catch (Exception ex)
            {
                await repository.RollbackTransection();
                throw ex;
            }
            finally
            {
                await repository.CloseConnectionAsync();
            }

        }
    }
}

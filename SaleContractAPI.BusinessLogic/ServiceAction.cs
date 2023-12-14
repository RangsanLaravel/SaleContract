using SaleContractAPI.DataAccess;
using SaleContractAPI.DataContract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace SaleContractAPI.BusinessLogic
{
    public class ServiceAction
    {
        #region " STATIC "
        private readonly string _connectionstring = string.Empty;
        private readonly CultureInfo culture = new CultureInfo("th-TH");
        private readonly string DBENV = string.Empty;
        public ServiceAction(string connectionstring, string DBENV)
        {
            this._connectionstring = connectionstring;
            this.DBENV = DBENV;
        }
        #endregion " STATIC "

        #region " GET "
        public async ValueTask<List<substatus>> GET_STATUS(string status_type)
        {
            List<substatus> dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                dataObjects = await repository.GET_STATUS(status_type);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await repository.CloseConnectionAsync();
            }
            return dataObjects;
        }

        public async ValueTask<List<Priority>> GET_PRIORITY()
        {
            List<Priority> dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                dataObjects = await repository.GET_PRIORITY();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await repository.CloseConnectionAsync();
            }
            return dataObjects;
        }
        public async ValueTask<List<TBT_SALE_STATUS>> GET_TBT_SALE_STATUS(int company_id)
        {
            List<TBT_SALE_STATUS> dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                dataObjects = await repository.GET_TBT_SALE_STATUS(company_id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await repository.CloseConnectionAsync();
            }
            return dataObjects;
        }

        public async ValueTask<List<company_detail>> GET_COMPANY(SEARCH_COMPANY condition)
        {
            List<company_detail> dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                dataObjects = await repository.GET_COMPANY(condition);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await repository.CloseConnectionAsync();
            }
            return dataObjects;
        }

        #endregion " GET "


        #region " INSERT "
        public async ValueTask INSERT_TBT_COMPANY_DETAIL(company_detail condition)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                await repository.INSERT_TBT_COMPANY_DETAIL(condition);
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

        public async ValueTask INSERT_TBT_SALE_STATUS(TBT_SALE_STATUS condition)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                await repository.INSERT_TBT_SALE_STATUS(condition);
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

        #endregion " INSERT "
    }
}

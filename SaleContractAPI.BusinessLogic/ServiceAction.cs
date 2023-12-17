using SaleContractAPI.DataAccess;
using SaleContractAPI.DataContract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                foreach (var item in dataObjects)
                {
                 var status =   await repository.GET_TBT_SALE_STATUS(Convert.ToInt32(item.ID));
                    if(!(status is  null))
                    {
                        item.Status = status.LastOrDefault().status_description;
                    }
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
               var companyid = await repository.INSERT_TBT_COMPANY_DETAIL(condition);
               await repository.INSERT_TBT_SALE_STATUS(new TBT_SALE_STATUS { company_id = companyid  
                    ,  status_code =condition.Status
                    ,fsystem_id =condition.Owner
                });
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
                if(condition.status_code == "WON")
                {
                    await repository.UPDATE_TBT_COMPANY_DETAIL_WON(condition.company_id.ToString());
                }
                await repository.UPDATE_TBT_COMPANY_DETAIL_STATUS(condition.status_code,condition.company_id.ToString());
                if (!string.IsNullOrWhiteSpace( condition.priority))
                {
                    await repository.UPDATE_TBT_COMPANY_DETAIL(new company_detail {ID=condition.company_id.ToString(),Priority=condition.priority });
                }
                if (!string.IsNullOrEmpty(condition.noti_dt))
                {
                    await repository.INSERT_TBT_SALE_NOTIFICATION(new tbt_sale_notification { companyid= condition.company_id.ToString(),noti_dt = DateTime.Parse(condition.noti_dt) });
                }
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

        #region " UPDATE "
        public async ValueTask UPDATE_TBT_SALE_NOTIFICATION(string  ID)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                await repository.UPDATE_TBT_SALE_NOTIFICATION(ID);
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
        #endregion " UPDATE "
    }
}

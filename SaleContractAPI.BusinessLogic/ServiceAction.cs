using Azure.Core.Diagnostics;
using OfficeOpenXml;
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
    public partial class ServiceAction
    {
        #region " STATIC "
        private readonly string _connectionstring = string.Empty;
        private readonly CultureInfo culture = new CultureInfo("th-TH");
        private readonly string DBENV = string.Empty;
        public ServiceAction(string connectionstring, string DBENV)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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
                if(dataObjects !=  null)
                {
                    foreach (var item in dataObjects)
                    {
                        var company =await repository.GET_COMPANY(new SEARCH_COMPANY { ID = company_id.ToString() });
                        if(company != null)
                        {
                            item.due_dt = company.FirstOrDefault().DealDateNoti;
                        }
                        
                        var remrk = await GET_REMARK(item.ID.ToString(),repository);
                        if(remrk != null && remrk.Count > 0)
                        {
                            item.remark_Statuses = new List<tbt_remark_status>();
                            item.remark_Statuses = remrk;
                        }
                        var getidremark =  await repository.GET_tbt_remark_status(item.ID.ToString());
                        if(getidremark != null)
                        {
                            item.ID = Convert.ToInt32(getidremark.FirstOrDefault().ID);
                        }
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
        public async ValueTask<List<TBT_SALE_STATUS>> GET_TBT_SALE_STATUS_JOB(int company_id)
        {
            List<TBT_SALE_STATUS> dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                dataObjects = await repository.GET_TBT_SALE_STATUS_JOB(company_id);
                if (dataObjects != null)
                {
                    foreach (var item in dataObjects)
                    {
                        var company = await repository.GET_COMPANY_JOB(new SEARCH_COMPANY { ID = company_id.ToString() });
                        if (company != null)
                        {
                            item.due_dt = company.FirstOrDefault().DealDateNoti;
                        }

                        var remrk = await GET_REMARK_JOB(item.ID.ToString(), repository);
                        if (remrk != null && remrk.Count > 0)
                        {
                            item.remark_Statuses = new List<tbt_remark_status>();
                            item.remark_Statuses = remrk;
                        }
                        var getidremark = await repository.GET_tbt_remark_status_JOB(item.ID.ToString());
                        if (getidremark != null)
                        {
                            item.ID = Convert.ToInt32(getidremark.FirstOrDefault().ID);
                        }
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

        private async ValueTask<List<tbt_remark_status>> GET_REMARK(string IDSTATUS, Repository repository)
        {
            List<tbt_remark_status> remark = new List<tbt_remark_status>();
            var mainremark = await repository.GET_tbt_remark_status(IDSTATUS);
            if (mainremark != null)
            {     
                var reply = await repository.GET_tbt_remark_status_UPLINE(mainremark.FirstOrDefault().ID);
                if (reply != null)
                    remark.AddRange(reply);
                else
                    return null;
                return remark;
            }
            else
                return null;
        }

        private async ValueTask<List<tbt_remark_status>> GET_REMARK_JOB(string IDSTATUS, Repository repository)
        {
            List<tbt_remark_status> remark = new List<tbt_remark_status>();
            var mainremark = await repository.GET_tbt_remark_status_JOB(IDSTATUS);
            if (mainremark != null)
            {
                var reply = await repository.GET_tbt_remark_status_UPLINE_JOB(mainremark.FirstOrDefault().ID);
                if (reply != null)
                    remark.AddRange(reply);
                else
                    return null;
                return remark;
            }
            else
                return null;
        }
        public async ValueTask<List<company_detail>> GET_COMPANY(SEARCH_COMPANY condition)
        {
            List<company_detail> dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                dataObjects = await repository.GET_COMPANY(condition);
                if (dataObjects is null)
                    return dataObjects;
                foreach (var item in dataObjects)
                {
                 var status =   await repository.GET_TBT_SALE_STATUS(Convert.ToInt32(item.ID));
                    if(!(status is  null))
                    {
                        item.Status = status.LastOrDefault().status_description;
                    }
                    //if (!string.IsNullOrEmpty(item.DealValue))
                    //{

                    //    item.Persen = (Convert.ToInt32(item.DealValue) * Convert.ToInt32(string.IsNullOrEmpty(item.Persen) ? 0 : Convert.ToInt32(item.Persen)) / 100).ToString();
                    //}
                    //else
                    //    item.Persen = "0";
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
        public async ValueTask<List<company_detail>> GET_COMPANY_JOB(SEARCH_COMPANY condition)
        {
            List<company_detail> dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                dataObjects = await repository.GET_COMPANY_JOB(condition);
                if (dataObjects is null)
                    return dataObjects;
                foreach (var item in dataObjects)
                {
                    var status = await repository.GET_TBT_SALE_STATUS_JOB(Convert.ToInt32(item.ID));
                    if (!(status is null))
                    {
                        item.Status = status.LastOrDefault().status_description;
                    }
                    //if (!string.IsNullOrEmpty(item.DealValue))
                    //{

                    //    item.Persen = (Convert.ToInt32(item.DealValue) * Convert.ToInt32(string.IsNullOrEmpty(item.Persen) ? 0 : Convert.ToInt32(item.Persen)) / 100).ToString();
                    //}
                    //else
                    //    item.Persen = "0";
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
        public async ValueTask<email_detail> GET_EMAILDETAIL(long UPLINEID)
        {
            email_detail dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
                getagain:
                var rmk = await repository.GET_IDSTATUS(UPLINEID);
                if(rmk?.ID_STATUS_SALE != null)
                {
                    dataObjects = new email_detail();
                    dataObjects = await repository.GET_STATUS_BY_ID(rmk.ID_STATUS_SALE);
                }
                else
                {
                    UPLINEID =Convert.ToInt64(rmk.ID_REMARK_UPLINE);
                    goto getagain;
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

        public async ValueTask<email_detail> GET_EMAILDETAIL_JOB(long UPLINEID)
        {
            email_detail dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            try
            {
            getagain:
                var rmk = await repository.GET_IDSTATUS_JOB(UPLINEID);
                if (rmk?.ID_STATUS_SALE != null)
                {
                    dataObjects = new email_detail();
                    dataObjects = await repository.GET_STATUS_BY_ID_JOB(rmk.ID_STATUS_SALE);
                }
                else
                {
                    UPLINEID = Convert.ToInt64(rmk.ID_REMARK_UPLINE);
                    goto getagain;
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
                var idstatussale=  await repository.INSERT_TBT_SALE_STATUS(new TBT_SALE_STATUS { company_id = companyid  
                    ,  status_code =condition.Status
                    ,fsystem_id =condition.Owner
                });
                await  repository.INSERT_TBT_REAMRK_STATUS(new tbt_remark_status
                {
                    ID_STATUS_SALE = idstatussale.ToString(),
                    REMARK_ID = condition.Owner
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
        public async ValueTask<UserInfo> INSERT_TBT_REAMRK_STATUS(tbt_remark_status condition)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            UserInfo userInfo = new UserInfo() ;
            try
            {
                /*if (!string.IsNullOrEmpty(condition.ID_STATUS_SALE))
                {
                    var idstatussale = await repository.CHECK_ID_STATUS_SALE(condition.ID_STATUS_SALE);
                    if(!(idstatussale is null))
                    {
                        condition.ID_REMARK_UPLINE = idstatussale.ID;
                        condition.ID_STATUS_SALE = string.Empty;
                    }
                }

                var rmkup =   await repository.INSERT_TBT_REAMRK_STATUS(condition);
                if(string.IsNullOrEmpty( condition.ID_STATUS_SALE))
                {
                    var rmk = await repository.CHECK_UPLINE_FOR_GORUP(Convert.ToInt64(condition.ID_REMARK_UPLINE));
                    if (rmk != null && !string.IsNullOrEmpty(rmk.ID_STATUS_SALE))
                    {
                        await repository.UPDATE_TBT_REAMRK_STATUS_GROUP(rmkup, rmkup);
                    }
                    else
                    {
                        await repository.UPDATE_TBT_REAMRK_STATUS_GROUP(Convert.ToInt64(condition.ord_group), rmkup);
                    }
                    
                }
                else
                {
                    await repository.UPDATE_TBT_REAMRK_STATUS_GROUP(rmkup, rmkup);
                }

                if (!string.IsNullOrEmpty(condition.ID_REMARK_UPLINE))
                    userInfo = await repository.GET_EMAIL(condition.ID_REMARK_UPLINE);
                else
                    userInfo = null;
                await repository.CommitTransection();
                return userInfo;*/
                var rmkup = await repository.INSERT_TBT_REAMRK_STATUS(condition);
                var rmk = await repository.CHECK_UPLINE_FOR_GORUP(Convert.ToInt64(condition.ID_REMARK_UPLINE));
                if (rmk != null && !string.IsNullOrEmpty(rmk.ID_STATUS_SALE))
                {
                    await repository.UPDATE_TBT_REAMRK_STATUS_GROUP(rmkup, rmkup);
                }
                else
                {
                    await repository.UPDATE_TBT_REAMRK_STATUS_GROUP(Convert.ToInt64(condition.ord_group), rmkup);
                }
                var userinfo = await repository.GET_EMAIL(condition.ID_REMARK_UPLINE);
                await repository.CommitTransection();
                return userinfo;
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
            var idstatus =    await repository.INSERT_TBT_SALE_STATUS(condition);
                if (!string.IsNullOrEmpty(condition.remark))
                {
                    tbt_remark_status remrk = new tbt_remark_status
                    {
                        ID_STATUS_SALE = idstatus.ToString(),
                        REMARK_ID =condition.fsystem_id,
                        REMARK = condition.remark
                    };
                   var uplineid =   await repository.INSERT_TBT_REAMRK_STATUS(remrk);
                    await repository.UPDATE_TBT_REAMRK_STATUS_GROUP(uplineid, uplineid);
                }
                if (condition.status_code == "WON")
                {
                    await repository.UPDATE_TBT_COMPANY_DETAIL_WON(condition.company_id.ToString());
                }
                if (!string.IsNullOrEmpty(condition.owner))
                {
                    await repository.UPDATE_OWNER(condition.company_id, condition.owner);
                }
                await repository.UPDATE_TBT_COMPANY_DETAIL_STATUS(condition.status_code,condition.company_id.ToString());
                //var companeydetail = new company_detail
                //{
                //    ID = condition.company_id.ToString(),
                //    NAME=condition.name,
                //    WEBSITE=condition.website,
                //    Contract =condition.contract,
                //    Email =condition.email,
                //    location =condition.Location,
                //    Priority = condition.priority,
                //    People =condition.People,
                //    Persen =condition.persen,
                //    Position=condition.position,
                //    DealValue =condition.Dealvalue,
                //    DealCreate =string.IsNullOrEmpty(condition.Dealcreationdate)? (DateTime?)null: DateTime.ParseExact(condition.Dealcreationdate, "dd/MM/yyyy",
                //                       System.Globalization.CultureInfo.InvariantCulture) ,
                //    DealDateFollowup = string.IsNullOrEmpty(condition.Duedatefollowup) ? (DateTime?)null : DateTime.ParseExact(condition.Duedatefollowup, "dd/MM/yyyy",
                //                       System.Globalization.CultureInfo.InvariantCulture),
                //    DealDateNoti = string.IsNullOrEmpty(condition.noti_dt) ? (DateTime?)null : DateTime.ParseExact(condition.noti_dt, "dd/MM/yyyy",
                //                       System.Globalization.CultureInfo.InvariantCulture),
                //    Mobile =condition.mobile,
                //    ModelType=condition.modelType

                //};
                await repository.UPDATE_TBT_COMPANY(condition);
                
                if (!string.IsNullOrEmpty(condition.noti_dt))
                {
                    await repository.INSERT_TBT_SALE_NOTIFICATION(new tbt_sale_notification { companyid= condition.company_id.ToString(),noti_dt = DateTime.ParseExact(condition.noti_dt, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture)
                    });
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

        public async ValueTask INSERT_TBT_COMPANY_DETAIL_JOB(company_detail condition)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                var companyid = await repository.INSERT_TBT_COMPANY_DETAIL_JOB(condition);
                var idstatussale = await repository.INSERT_TBT_SALE_STATUS_JOB(new TBT_SALE_STATUS
                {
                    company_id = companyid
                    ,
                    status_code = condition.Status
                    ,
                    fsystem_id = condition.Owner
                });
                await repository.INSERT_TBT_REAMRK_STATUS(new tbt_remark_status
                {
                    ID_STATUS_SALE = idstatussale.ToString(),
                    REMARK_ID = condition.Owner
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
        public async ValueTask<UserInfo> INSERT_TBT_REAMRK_STATUS_JOB(tbt_remark_status condition)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            UserInfo userInfo = new UserInfo();
            try
            {
                /*if (!string.IsNullOrEmpty(condition.ID_STATUS_SALE))
                {
                    var idstatussale = await repository.CHECK_ID_STATUS_SALE(condition.ID_STATUS_SALE);
                    if(!(idstatussale is null))
                    {
                        condition.ID_REMARK_UPLINE = idstatussale.ID;
                        condition.ID_STATUS_SALE = string.Empty;
                    }
                }

                var rmkup =   await repository.INSERT_TBT_REAMRK_STATUS(condition);
                if(string.IsNullOrEmpty( condition.ID_STATUS_SALE))
                {
                    var rmk = await repository.CHECK_UPLINE_FOR_GORUP(Convert.ToInt64(condition.ID_REMARK_UPLINE));
                    if (rmk != null && !string.IsNullOrEmpty(rmk.ID_STATUS_SALE))
                    {
                        await repository.UPDATE_TBT_REAMRK_STATUS_GROUP(rmkup, rmkup);
                    }
                    else
                    {
                        await repository.UPDATE_TBT_REAMRK_STATUS_GROUP(Convert.ToInt64(condition.ord_group), rmkup);
                    }
                    
                }
                else
                {
                    await repository.UPDATE_TBT_REAMRK_STATUS_GROUP(rmkup, rmkup);
                }

                if (!string.IsNullOrEmpty(condition.ID_REMARK_UPLINE))
                    userInfo = await repository.GET_EMAIL(condition.ID_REMARK_UPLINE);
                else
                    userInfo = null;
                await repository.CommitTransection();
                return userInfo;*/
                var rmkup = await repository.INSERT_TBT_REAMRK_STATUS_JOB(condition);
                var rmk = await repository.CHECK_UPLINE_FOR_GORUP_JOB(Convert.ToInt64(condition.ID_REMARK_UPLINE));
                if (rmk != null && !string.IsNullOrEmpty(rmk.ID_STATUS_SALE))
                {
                    await repository.UPDATE_TBT_REAMRK_STATUS_GROUP_JOB(rmkup, rmkup);
                }
                else
                {
                    await repository.UPDATE_TBT_REAMRK_STATUS_GROUP_JOB(Convert.ToInt64(condition.ord_group), rmkup);
                }
                var userinfo = await repository.GET_EMAIL_JOB(condition.ID_REMARK_UPLINE);
                await repository.CommitTransection();
                return userinfo;
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
        public async ValueTask INSERT_TBT_SALE_STATUS_JOB(TBT_SALE_STATUS condition)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                var idstatus = await repository.INSERT_TBT_SALE_STATUS_JOB(condition);
                if (!string.IsNullOrEmpty(condition.remark))
                {
                    tbt_remark_status remrk = new tbt_remark_status
                    {
                        ID_STATUS_SALE = idstatus.ToString(),
                        REMARK_ID = condition.fsystem_id,
                        REMARK = condition.remark
                    };
                    var uplineid = await repository.INSERT_TBT_REAMRK_STATUS_JOB(remrk);
                    await repository.UPDATE_TBT_REAMRK_STATUS_GROUP_JOB(uplineid, uplineid);
                }
                if (condition.status_code == "WON")
                {
                    await repository.UPDATE_TBT_COMPANY_DETAIL_WON_JOB(condition.company_id.ToString());
                }
                if (!string.IsNullOrEmpty(condition.owner))
                {
                    await repository.UPDATE_OWNER(condition.company_id, condition.owner);
                }
                await repository.UPDATE_TBT_COMPANY_DETAIL_STATUS_JOB(condition.status_code, condition.company_id.ToString());
                //var companeydetail = new company_detail
                //{
                //    ID = condition.company_id.ToString(),
                //    NAME=condition.name,
                //    WEBSITE=condition.website,
                //    Contract =condition.contract,
                //    Email =condition.email,
                //    location =condition.Location,
                //    Priority = condition.priority,
                //    People =condition.People,
                //    Persen =condition.persen,
                //    Position=condition.position,
                //    DealValue =condition.Dealvalue,
                //    DealCreate =string.IsNullOrEmpty(condition.Dealcreationdate)? (DateTime?)null: DateTime.ParseExact(condition.Dealcreationdate, "dd/MM/yyyy",
                //                       System.Globalization.CultureInfo.InvariantCulture) ,
                //    DealDateFollowup = string.IsNullOrEmpty(condition.Duedatefollowup) ? (DateTime?)null : DateTime.ParseExact(condition.Duedatefollowup, "dd/MM/yyyy",
                //                       System.Globalization.CultureInfo.InvariantCulture),
                //    DealDateNoti = string.IsNullOrEmpty(condition.noti_dt) ? (DateTime?)null : DateTime.ParseExact(condition.noti_dt, "dd/MM/yyyy",
                //                       System.Globalization.CultureInfo.InvariantCulture),
                //    Mobile =condition.mobile,
                //    ModelType=condition.modelType

                //};
                await repository.UPDATE_TBT_COMPANY_JOB(condition);

                if (!string.IsNullOrEmpty(condition.noti_dt))
                {
                    await repository.INSERT_TBT_SALE_NOTIFICATION_JOB(new tbt_sale_notification
                    {
                        companyid = condition.company_id.ToString(),
                        noti_dt = DateTime.ParseExact(condition.noti_dt, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture)
                    });
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

        public async ValueTask UPDATE_TBT_SALE_NOTIFICATION_JOB(string ID)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                await repository.UPDATE_TBT_SALE_NOTIFICATION_JOB(ID);
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

        #region " PROCEDURE "
        public async ValueTask SP_DUPLICATE_COMPANY(string ID, string USERID)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                await repository.SP_DUPLICATE_COMPANY(ID,USERID);
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

        public async ValueTask SP_DUPLICATE_COMPANY_JOB(string ID, string USERID)
        {
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            await repository.beginTransection();
            try
            {
                await repository.SP_DUPLICATE_COMPANY_JOB(ID, USERID);
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
        public async ValueTask<List<company_detail>> SP_GET_COMPANY_WON(string dateTimest, string dateTimeen,string userid)
        {
            List<company_detail> dataObjects = null;
            Repository repository = new Repository(_connectionstring, DBENV);
            await repository.OpenConnectionAsync();
            DateTime? dtst = null;
            DateTime? dten = null;
            try
            {
                if (!string.IsNullOrEmpty(dateTimest))
                {
                    dtst = DateTime.ParseExact(dateTimest, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTimeen))
                {
                    dten = DateTime.ParseExact(dateTimeen, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                }
                dataObjects = await repository.SP_GET_COMPANY_WON(dtst, dten, userid);
                if (dataObjects is null)
                    return dataObjects;
                foreach (var item in dataObjects)
                {
                    var status = await repository.GET_TBT_SALE_STATUS(Convert.ToInt32(item.ID));
                    if (!(status is null))
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
        #endregion " PROCEDURE "
    }
}

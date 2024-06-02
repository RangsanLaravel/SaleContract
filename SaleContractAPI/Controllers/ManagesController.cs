using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SaleContractAPI.BusinessLogic;
using SaleContractAPI.DataContract;
using System.Security.Claims;

namespace SaleContractAPI.Controllers
{
    //[Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ManagesController : ControllerBase
    {
        private readonly IConfiguration Configuration = null;
        private readonly ServiceAction service;
        //private readonly string _userid;
        public ManagesController(IConfiguration Configuration)
        {
            
            this.Configuration = Configuration;
            this.service = new ServiceAction(this.Configuration.GetConnectionString("ConnectionSQLServer"), Configuration["ConfigSetting:DBENV"]);
        }

        [HttpGet("GET_STATUS/{status_type}")]
        public async ValueTask<IActionResult> GET_STATUS(string status_type)
        {
            try
            {
                var result = await this.service.GET_STATUS(status_type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GET_PRIORITY")]
        public async ValueTask<IActionResult> GET_PRIORITY()
        {
            try
            {
                var result = await this.service.GET_PRIORITY();
                if (result is null)
                    return Ok();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GET_TBT_SALE_STATUS/{companyid}")]
        public async ValueTask<IActionResult> GET_TBT_SALE_STATUS(int companyid)
        {
            try
            {
                var result = await this.service.GET_TBT_SALE_STATUS(companyid);
                if (result is null)
                    return Ok();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GET_TBT_SALE_STATUS_JOB/{companyid}")]
        public async ValueTask<IActionResult> GET_TBT_SALE_STATUS_JOB(int companyid)
        {
            try
            {
                var result = await this.service.GET_TBT_SALE_STATUS_JOB(companyid);
                if (result is null)
                    return Ok();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("GET_COMPANY")]
        public async ValueTask<IActionResult> GET_COMPANY(SEARCH_COMPANY condition)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                var _position = User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).FirstOrDefault();
                if (_position == "CK")
                {
                    condition.Owner = string.Empty;
                }
                else
                {
                    condition.Owner = _userid;
                }
                var result = await this.service.GET_COMPANY(condition);
                if (result is null)
                    return NoContent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("GET_COMPANY_JOB")]
        public async ValueTask<IActionResult> GET_COMPANY_JOB(SEARCH_COMPANY condition)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                var _position = User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).FirstOrDefault();
                if (_position == "CK")
                {
                    condition.Owner = string.Empty;
                }
                else
                {
                    condition.Owner = _userid;
                }
                var result = await this.service.GET_COMPANY_JOB(condition);
                if (result is null)
                    return NoContent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("INSERT_TBT_COMPANY_DETAIL")]
 
        public async ValueTask<IActionResult> INSERT_TBT_COMPANY_DETAIL(company_detail condition)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                condition.Owner = _userid;
                await this.service.INSERT_TBT_COMPANY_DETAIL(condition);
                
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("INSERT_TBT_COMPANY_DETAIL_JOB")]

        public async ValueTask<IActionResult> INSERT_TBT_COMPANY_DETAIL_JOB(company_detail condition)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                condition.Owner = _userid;
                await this.service.INSERT_TBT_COMPANY_DETAIL_JOB(condition);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("INSERT_TBT_SALE_STATUS")]
        public async ValueTask<IActionResult> INSERT_TBT_SALE_STATUS(TBT_SALE_STATUS condition)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                condition.fsystem_id = _userid;
                await this.service.INSERT_TBT_SALE_STATUS(condition);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}::${ex.StackTrace}");
            }
        }

        [HttpPost("INSERT_TBT_SALE_STATUS_JOB")]
        public async ValueTask<IActionResult> INSERT_TBT_SALE_STATUS_JOB(TBT_SALE_STATUS condition)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                condition.fsystem_id = _userid;
                await this.service.INSERT_TBT_SALE_STATUS_JOB(condition);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}::${ex.StackTrace}");
            }
        }

        [HttpPost("INSERT_TBT_REAMRK_STATUS")]
        public async ValueTask<IActionResult> INSERT_TBT_REAMRK_STATUS(tbt_remark_status condition)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                condition.REMARK_ID = _userid;
                var userinfo =  await this.service.INSERT_TBT_REAMRK_STATUS(condition);
                if(userinfo is null)
                {
                    return Ok();
                }
                return Ok(userinfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("INSERT_TBT_REAMRK_STATUS_JOB")]
        public async ValueTask<IActionResult> INSERT_TBT_REAMRK_STATUS_JOB(tbt_remark_status condition)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                condition.REMARK_ID = _userid;
                var userinfo = await this.service.INSERT_TBT_REAMRK_STATUS_JOB(condition);
                if (userinfo is null)
                {
                    return Ok();
                }
                return Ok(userinfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GET_EMAILDETAIL/{UPLINEID}")]
        public async ValueTask<IActionResult> GET_EMAILDETAIL(long UPLINEID)
        {
            try
            {
            var result=    await this.service.GET_EMAILDETAIL(UPLINEID);
                if(result is null)
                    return Ok();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GET_EMAILDETAIL_JOB/{UPLINEID}")]
        public async ValueTask<IActionResult> GET_EMAILDETAIL_JOB(long UPLINEID)
        {
            try
            {
                var result = await this.service.GET_EMAILDETAIL_JOB(UPLINEID);
                if (result is null)
                    return Ok();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ACCAPT/{ID}")]
        public async ValueTask<IActionResult> ACCAPT_NOTIFICATION(string ID)
        {
            try
            {

                await this.service.UPDATE_TBT_SALE_NOTIFICATION(ID);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ACCAPT_JOB/{ID}")]
        public async ValueTask<IActionResult> ACCAPT_NOTIFICATION_JOB(string ID)
        {
            try
            {

                await this.service.UPDATE_TBT_SALE_NOTIFICATION_JOB(ID);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("SP_DUPLICATE_COMPANY/{ID}")]
        public async ValueTask<IActionResult> SP_DUPLICATE_COMPANY(string ID)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                await this.service.SP_DUPLICATE_COMPANY(ID, _userid);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("SP_DUPLICATE_COMPANY_JOB/{ID}")]
        public async ValueTask<IActionResult> SP_DUPLICATE_COMPANY_JOB(string ID)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                await this.service.SP_DUPLICATE_COMPANY_JOB(ID, _userid);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SP_GET_COMPANY_WON")]
        public async ValueTask<IActionResult> SP_GET_COMPANY_WON(SEARCH_COMPANY_WON condition)
        {
            try
            {
                var result=  await this.service.SP_GET_COMPANY_WON(condition.dateTimest, condition.dateTimeen);
                if (result is null)
                    return NoContent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("SP_GET_REPORT_CRM_BY_SALE")]
        public async ValueTask<IActionResult> SP_GET_REPORT_CRM_BY_SALE()
        {
            try
            {
                var result = await this.service.SP_GET_REPORT_CRM_BY_SALE();
                if (result is null)
                    return NoContent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("SP_GET_REPORT_CRM_BY_STATUS_SALE")]
        public async ValueTask<IActionResult> SP_GET_REPORT_CRM_BY_STATUS_SALE()
        {
            try
            {
                var result = await this.service.SP_GET_REPORT_CRM_BY_STATUS_SALE();
                if (result is null)
                    return NoContent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SP_GET_REPORT_CRM_BY_SERVICE")]
        public async ValueTask<IActionResult> SP_GET_REPORT_CRM_BY_SERVICE()
        {
            try
            {
                var result = await this.service.SP_GET_REPORT_CRM_BY_SERVICE();
                if (result is null)
                    return NoContent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

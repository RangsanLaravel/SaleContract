using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SaleContractAPI.BusinessLogic;
using SaleContractAPI.DataContract;

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

        [HttpPost("GET_COMPANY")]
        public async ValueTask<IActionResult> GET_COMPANY(SEARCH_COMPANY condition)
        {
            try
            {
                var result = await this.service.GET_COMPANY(condition);
                if (result is null)
                    return Ok();
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

        [HttpPost("INSERT_TBT_REAMRK_STATUS")]
        public async ValueTask<IActionResult> INSERT_TBT_REAMRK_STATUS(tbt_remark_status condition)
        {
            try
            {
                var _userid = User.Claims.Where(a => a.Type == "id").Select(a => a.Value).FirstOrDefault();
                condition.REMARK_ID = _userid;
                var userinfo =  await this.service.INSERT_TBT_REAMRK_STATUS(condition);
                
                return Ok(userinfo);
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
    }
}

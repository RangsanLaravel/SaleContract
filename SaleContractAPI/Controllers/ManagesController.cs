using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaleContractAPI.BusinessLogic;
using SaleContractAPI.DataContract;

namespace SaleContractAPI.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ManagesController : ControllerBase
    {
        private readonly IConfiguration Configuration = null;
        private readonly ServiceAction service;
        public ManagesController(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            this.service = new ServiceAction(this.Configuration.GetConnectionString("ConnectionSQLServer"), Configuration["ConfigSetting:DBENV"]);
        }

        [HttpGet("GET_STATUS/{status_type}")]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async ValueTask<IActionResult> GET_PRIORITY()
        {
            try
            {
                var result = await this.service.GET_PRIORITY();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GET_TBT_SALE_STATUS/{companyid}")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> GET_TBT_SALE_STATUS(int companyid)
        {
            try
            {
                var result = await this.service.GET_TBT_SALE_STATUS(companyid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("GET_COMPANY")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> GET_COMPANY(SEARCH_COMPANY condition)
        {
            try
            {
                var result = await this.service.GET_COMPANY(condition);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("INSERT_TBT_COMPANY_DETAIL")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> INSERT_TBT_COMPANY_DETAIL(company_detail condition)
        {
            try
            {
                await this.service.INSERT_TBT_COMPANY_DETAIL(condition);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}

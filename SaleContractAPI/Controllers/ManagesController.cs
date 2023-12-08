using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SaleContractAPI.Controllers
{
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
            //this.mailService = mailService;
        }
    }
}

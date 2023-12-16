using Microsoft.AspNetCore.Mvc;
using RestSharp;
using SaleContract.Models;
using SaleContractAPI.DataContract;
using System.Diagnostics;
using System.Text.Json;

namespace SaleContract.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public IActionResult Index(SEARCH_COMPANY condition)
        {
            //    return RedirectToAction("Logout");
            RestClient client = new RestClient(_configuration["API:ISEECENTER"]);
            RestRequest request = new RestRequest($"api/Monitors/GET_DETAIL_ALLJOB", Method.Post);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var isNulldata = condition.NAME is null
                            && condition.MOBILE is null
                            && condition.Status is null
                            && condition.EMAIL is null
                            && condition.Priority is null
                            ;
            if (isNulldata)
            {
                condition = new SEARCH_COMPANY
                {
                    limit = "50",
                    NAME = string.Empty,
                    Status = string.Empty,
                    MOBILE = string.Empty,
                    EMAIL = string.Empty,
                    Priority = string.Empty
                };
            }
            else
            {
                condition.NAME = condition.NAME ?? string.Empty;
                condition.Status = condition.Status ?? string.Empty;
                condition.Priority = condition.Priority ?? string.Empty;
                condition.MOBILE = condition.MOBILE ?? string.Empty;
                condition.EMAIL = condition.EMAIL ?? string.Empty;
                condition.limit = condition.limit ?? "100";
            }
            request.AddBody(condition);
            var response = client.Execute<List<company_detail>>(request);
            ViewBag.Fullname = HttpContext.Session.GetString("fullname");
            ViewData["priority"] = GET_PRIORITY();
            ViewData["substatus"] = GET_STATUS();
          //  ViewBag.Substatus = HttpContext.Session.GetString("substatus") == null ? null : JsonSerializer.Deserialize<List<substatus>>(HttpContext.Session.GetString("substatus"));
           // ViewBag.priority = HttpContext.Session.GetString("priority") == null ? null : JsonSerializer.Deserialize<List<Priority>>(HttpContext.Session.GetString("priority"));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return View(response?.Data);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return View();
            }
            else
            {
                ViewBag.Error = response.Content;
                return View("Error");
            }
        }

        public IActionResult Link(string token)
        {
            GET_USERINFO(token);
            // GET_OWNERID();
            //GET_TBM_SUBSTATUS();
            HttpContext.Session.SetString("token", token);
            return RedirectToAction("Index", "Home", null);
        }

        protected void GET_USERINFO(string token)
        {
            RestClient client = new RestClient(_configuration["API:ISEESERVICE"]);
            RestRequest request = new RestRequest($"api/v1/ISEEServices/Userinfo", Method.Post);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute<UserInfo>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContext.Session.SetString("userinfo", JsonSerializer.Serialize(response.Data));
                HttpContext.Session.SetString("fullname", response.Data.fullname);
            }
        }

        protected List<substatus> GET_STATUS()
        {
            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"api/v1/Manages/GET_STATUS/SALE", Method.Get);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var response = client.Execute<List<substatus>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContext.Session.SetString("substatus", JsonSerializer.Serialize(response.Data));
            }
            return response.Data;
        }

        protected List<Priority> GET_PRIORITY()
        {
            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"api/v1/Manages/GET_PRIORITY", Method.Get);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var response = client.Execute<List<Priority>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContext.Session.SetString("priority", JsonSerializer.Serialize(response.Data));            
            }
            return response.Data;
        }


        public async ValueTask<IActionResult> Create(company_detail company)
        {
            if (!ModelState.IsValid)
            {
                return View(company);
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
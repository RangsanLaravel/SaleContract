using Microsoft.AspNetCore.Mvc;
using RestSharp;
using SaleContract.Models;
using SaleContractAPI.DataContract;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            if (HttpContext.Session.GetString("token") is null)
                return RedirectToAction("Logout");
            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"api/v1/Manages/GET_COMPANY", Method.Post);
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
                    Priority = string.Empty,
                    ID = string.Empty,
                    Owner = string.Empty,
                    DealDateFollowup =string.Empty
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
                condition.ID = string.Empty;
                condition.Owner = string.Empty;
                condition.DealDateFollowup = string.Empty;
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

        [HttpGet]
        public IActionResult GET_TIMELINE(string companyid)
        {
            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            var request = new RestRequest($"api/v1/Manages/GET_TBT_SALE_STATUS/{companyid}", Method.Get);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var response = client.Execute<List<TBT_SALE_STATUS>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(new { success = true, data = response.Data });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Json(new { success = true, data = "" });
            }
            return Json(new { success = false, error = response.Content });

        }

        [HttpPost]
        public IActionResult UpdateStatus(TBT_SALE_STATUS data)
        {
            if (HttpContext.Session.GetString("token") is null)
                return RedirectToAction("Logout");
            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"/api/v1/Manages/INSERT_TBT_SALE_STATUS", Method.Post);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            data.priority = data.priority ?? string.Empty;
            data.remark = data.remark ?? string.Empty;
            data.tmn_flg = string.Empty;
            data.status_description = string.Empty;
            data.fsystem_id = string.Empty;
            request.AddJsonBody(data);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, error = response.Content });
        }

        public IActionResult Create(company_detail company)
        {        
            company.Priority_description = "1234";
            company.Owner = "1234";
            company.ID = "1234";
            company.LastUpdate = DateTime.Now;
            company.DealCreate = DateTime.Now;
            company.DealDateNoti = company.DealDateFollowup!=null? company.DealDateFollowup.Value.AddDays(-2):null;
            if (string.IsNullOrEmpty(company.NAME))
            {
                ViewData["priority"] = GET_PRIORITY();
                ViewData["substatus"] = GET_STATUS();
                return View(company);
            }
            else
            {
                RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
                RestRequest request = new RestRequest($"/api/v1/Manages/INSERT_TBT_COMPANY_DETAIL", Method.Post);
                request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));               
                request.AddJsonBody(company);
                var response = client.Execute<List<Priority>>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = response.Content;
                return View("Error");
            }

           // return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userinfo");
            HttpContext.Session.Remove("fullname");
            HttpContext.Session.Remove("token");
            return Redirect(_configuration["Link:Login"]);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
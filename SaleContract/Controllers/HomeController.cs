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
                    limit = "1000",
                    NAME = string.Empty,
                    Status = string.Empty,
                    MOBILE = string.Empty,
                    EMAIL = string.Empty,
                    Priority = string.Empty,
                    ID = string.Empty,
                    Owner = string.Empty,
                    DealDateFollowup =string.Empty,
                    Contract = string.Empty,
                Remark =  string.Empty,
                ModelType =  string.Empty
            };
            }
            else
            {
                condition.NAME = condition.NAME ?? string.Empty;
                condition.Status = condition.Status ?? string.Empty;
                condition.Priority = condition.Priority ?? string.Empty;
                condition.MOBILE = condition.MOBILE ?? string.Empty;
                condition.EMAIL = condition.EMAIL ?? string.Empty;
                condition.Contract = condition.Contract ?? string.Empty;
                condition.Remark = condition.Remark ?? string.Empty;
                condition.ModelType = condition.ModelType ?? string.Empty;
                condition.limit = condition.limit ?? "1000";
                condition.ID = string.Empty;
                condition.Owner = string.Empty;
                condition.DealDateFollowup = string.Empty;
            }
            request.AddBody(condition);
            var response = client.Execute<List<company_detail>>(request);
            ViewBag.Fullname = HttpContext.Session.GetString("fullname");
            ViewData["priority"] = GET_PRIORITY();
            ViewData["substatus"] = GET_STATUS();
            ViewData["employee"] = GET_EMPLOYEE();
            //ViewData["position"] = "CK";
            ViewData["position"] = HttpContext.Session.GetString("position");
            //  ViewBag.Substatus = HttpContext.Session.GetString("substatus") == null ? null : JsonSerializer.Deserialize<List<substatus>>(HttpContext.Session.GetString("substatus"));
            // ViewBag.priority = HttpContext.Session.GetString("priority") == null ? null : JsonSerializer.Deserialize<List<Priority>>(HttpContext.Session.GetString("priority"));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                ViewData["sumarypriority"] = @$"<p><strong>Hight : </strong>{(response?.Data == null ? 0 : response.Data.Where(a => a.Priority == "1")?.Count()
                    )} <strong>Medium :</strong>{(response?.Data == null ? 0 : response.Data.Where(a => a.Priority == "2")?.Count())} <strong>Low :</strong>{(response?.Data == null ? 0 : response.Data.Where(a => a.Priority == "3")?.Count())} 
                      <strong> DealValue :</strong>{(response?.Data == null ? 0 : response.Data.Select(a => { a.DealValue = a.DealValue ??"0"; return a; }).Sum(a=>Convert.ToInt64(a.DealValue)))}</p>";
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
                HttpContext.Session.SetString("position", response.Data.position);
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

        protected List<employee> GET_EMPLOYEE()
        {
            RestClient client = new RestClient(_configuration["API:ISEESERVICE"]);
            RestRequest request = new RestRequest($"api/v1/ISEEServices/GET_EMPLOYEE", Method.Get);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var response = client.Execute<List<employee>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContext.Session.SetString("employee", JsonSerializer.Serialize(response.Data));
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
            try
            {
                RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
                RestRequest request = new RestRequest($"/api/v1/Manages/INSERT_TBT_SALE_STATUS", Method.Post);
                request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
                data.priority = data.priority ?? string.Empty;
                data.name = data.name ?? string.Empty;
                data.website = data.website ?? string.Empty;
                data.contract = data.contract ?? string.Empty;
                data.remark = data.remark ?? string.Empty;
                data.tmn_flg = string.Empty;
                data.status_description = string.Empty;
                data.persen = data.persen ?? string.Empty;
                data.Location = data.Location ?? string.Empty;
                data.position = data.position ?? string.Empty;
                data.modelType = data.modelType ?? string.Empty;
                data.mobile = data.mobile ?? string.Empty;
                data.email = data.email ?? string.Empty;
                data.owner = data.owner ?? string.Empty;
                data.People = data.People ?? string.Empty;
                data.Dealvalue = data.Dealvalue ?? string.Empty;
                data.Dealcreationdate = data.Dealcreationdate ?? string.Empty;
                data.Duedatefollowup = data.Duedatefollowup ?? string.Empty;
                data.noti_dt = data.noti_dt ?? string.Empty;
                data.fsystem_id = string.Empty;
                data.remark_Statuses = new List<tbt_remark_status>();
                request.AddJsonBody(data);
                var response = client.Execute<UserInfo>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(response?.Data?.email))
                    {
                        email_service email = new email_service
                        {
                            email = response.Data.email,
                            subject = $"Message From {data.name}",
                            body = $"Message From {data.name} Check in Remark"
                        };

                        SendEmail(email);
                    }
                    return Json(new { success = true });
                }
                throw new Exception(response.Content);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = $"{ex.Message} ::{ex.StackTrace}"  });
            }
                     
           
        }

        [HttpPost("INSERT_REMARK_REPLY")]
        public IActionResult INSERT_REMARK_REPLY(tbt_remark_status data)
        {
            if (HttpContext.Session.GetString("token") is null)
                return RedirectToAction("Logout");
            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"/api/v1/Manages/INSERT_TBT_REAMRK_STATUS", Method.Post);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            data.TMN_FLG = "N";
            data.REMARK_ID  =  string.Empty;
            data.ID_STATUS_SALE = string.IsNullOrEmpty(data.ID_STATUS_SALE)? string.Empty : data.ID_STATUS_SALE;
            data.REMARK_DT = string.Empty;
            data.ID = string.Empty;
            data.ID_REMARK_UPLINE = string.IsNullOrEmpty(data.ID_REMARK_UPLINE) ? string.Empty : data.ID_REMARK_UPLINE;
            data.ord_group = data.ord_group == "undefined" ? "": data.ord_group;
            data.ord_group =string.IsNullOrEmpty( data.ord_group ) ? "" : data.ord_group;
            request.AddJsonBody(data);
            try
            {
                var response = client.Execute<UserInfo>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(response?.Data?.email))
                    {
                        var emaildt = GET_EMAILDETAIL(Convert.ToInt64(data.ID_REMARK_UPLINE));
                        email_service email = new email_service
                        {
                            email = response.Data.email,
                            subject = $"Message From {response.Data?.fullname}",
                            body = $@"<span>ลูกค้า:{emaildt.customername}</span>
                                  <span>Status:{emaildt.status}</span>
                                  <span>Remark:-</span>
                                  <span>Reply:{data.REMARK}</span>
                                "
                        };

                        SendEmail(email);
                    }
                    return Json(new { success = true });
                }
                return Json(new { success = false, error = response.Content });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
           
        }
        public IActionResult Create(company_detail company)
        {
            ViewBag.Fullname = HttpContext.Session.GetString("fullname");
            company.Priority_description = "1234";
            company.Owner = "1234";
            company.Ownerid = "1234";
            company.Won = "1234";
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

        private async ValueTask SendEmail(email_service email)
        {
            RestClient client = new RestClient(_configuration["API:ISEESERVICE"]);
            RestRequest request = new RestRequest($"/api/v1/ISEEServices/SendEmail", Method.Post);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            request.AddJsonBody(email);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var a = 1;
            }
        }

        private email_detail GET_EMAILDETAIL(long UPLINEID)
        {
            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"/api/v1/Manages/GET_EMAILDETAIL/{UPLINEID}", Method.Get);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var response = client.Execute<email_detail>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data;
            }
            return null;
        }
        [HttpPost]
        public IActionResult DuplicateData(SEARCH_COMPANY companyid)
        {

            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"/api/v1/Manages/SP_DUPLICATE_COMPANY/{companyid.ID}", Method.Get);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, error = response.Content });
            }


            // return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult SP_GET_REPORT_CRM_BY_SALE()
        {

            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"/api/v1/Manages/SP_GET_REPORT_CRM_BY_SALE", Method.Post);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var response = client.Execute<string>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var database64 = response.Data;
                if(database64 != null)
                {
                    return File(Convert.FromBase64String(database64), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
                else
                {
                    ViewBag.Error = "ไม่พบไฟล์";
                    return View("Error");
                }
                
            }
            else
            {
                ViewBag.Error = response.Content;
                return View("Error");
            }


            // return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult SP_GET_REPORT_CRM_BY_STATUS_SALE()
        {

            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"/api/v1/Manages/SP_GET_REPORT_CRM_BY_STATUS_SALE", Method.Post);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var response = client.Execute<string>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var database64 = response.Data;
                if (database64 != null)
                {
                    return File(Convert.FromBase64String(database64), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
                else
                {
                    ViewBag.Error = "ไม่พบไฟล์";
                    return View("Error");
                }

            }
            else
            {
                ViewBag.Error = response.Content;
                return View("Error");
            }

            // return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult TERMINATE_TBT_COMPANY_DETAIL(SEARCH_COMPANY companyid)
        {
            RestClient client = new RestClient(_configuration["API:SALECONTRACTAPI"]);
            RestRequest request = new RestRequest($"/api/v1/Manages/TERMINATE_TBT_COMPANY_DETAIL/{companyid.ID}", Method.Delete);
            request.AddHeader("Authorization", "Bearer " + HttpContext.Session.GetString("token"));
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, error = response.Content });
            }
        }
    }
}
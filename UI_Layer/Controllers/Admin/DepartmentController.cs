using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using UI_Layer.Dtos.AnnouncementDto;
using UI_Layer.Dtos.DepartmentDto;
using UI_Layer.Models;
using UI_Layer.ValidationRules.Department;

namespace UI_Layer.Controllers.Admin
{
    [Authorize(Policy = "AdminPolicy")]
    [Route("/Department/{otherDepartmentId}")]
    public class DepartmentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DepartmentController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> DepartmentList()
        {
            var accessToken = HttpContext.Request.Cookies["jwt"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var responseMessage = await client.GetAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Department");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultDepartmentDto>>(jsonData);
                return View(values);
            }
            return View();
        }

        [HttpGet("Add")]
        public IActionResult AddDepartment()
        {
            return View();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddDepartment(CreateDepartmentDto newDepartment)
        {
            CreateDepartmentValidator validations = new CreateDepartmentValidator();
            ValidationResult results = validations.Validate(newDepartment);
            if (results.IsValid)
            {
                var accessToken = HttpContext.Request.Cookies["AuthenticationToken"];
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var jsonData = JsonConvert.SerializeObject(newDepartment);
                StringContent jsonAnnouncement = new(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Department", jsonAnnouncement);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseBody = await responseMessage.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<EmployeeManagerResponse>(responseBody);
                    if (responseObject.IsSuccess)
                    {
                        return RedirectToAction("Index", "Department");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid email or password");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please try again later.");
                    return View();
                }
            }
            return View();
        }

        [Route("Details/{departmentId}")]
        [HttpGet]
        public async Task<IActionResult> DepartmentDetails(int departmentId)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Department/{departmentId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<ResultAnnouncementDto>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}
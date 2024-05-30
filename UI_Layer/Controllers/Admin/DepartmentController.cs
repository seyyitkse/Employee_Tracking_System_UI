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
            var accessToken = HttpContext.Request.Cookies["AuthenticationToken"];
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

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AddDepartment()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AddDepartment(CreateDepartmentDto newDepartment)
        {
            CreateDepartmentValidator validations = new CreateDepartmentValidator();
            ValidationResult results = validations.Validate(newDepartment);
            if (results.IsValid)
            {
                var accessToken = HttpContext.Request.Cookies["AuthenticationToken"];
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                newDepartment.Status = true;
                var jsonData = JsonConvert.SerializeObject(newDepartment);
                StringContent jsonDepartment = new(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Department", jsonDepartment);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseBody = await responseMessage.Content.ReadAsStringAsync();
                      
                        return RedirectToAction("DepartmentList", "Department");
                        
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please try again later.");
                    return View();
                }
            }
            return View();
        }
        [HttpPut]
        public async Task<IActionResult> CloseDepartment(int id)
        {
            var accessToken = HttpContext.Request.Cookies["AuthenticationToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var responseMessage = await client.PutAsync($"https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Department/{id}", null);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("DepartmentList");
            }
            return View("Error");
        }
        [HttpGet]
        public async Task<IActionResult> DepartmentDetails(int id)
        {
            var accessToken = HttpContext.Request.Cookies["AuthenticationToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var responseMessage = await client.GetAsync($"https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Department/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<ResultDepartmentDto>(jsonData);
                return View(department);
            }
            return View();
        }
    }
}
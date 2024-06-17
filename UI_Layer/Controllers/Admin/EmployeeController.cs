using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using UI_Layer.Dtos.DepartmentDto;
using UI_Layer.Dtos.EmployeeDto;
using UI_Layer.ValidationRules.Employee;

namespace UI_Layer.Controllers.Admin
{

    public class EmployeeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public EmployeeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task <IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Employee");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultEmployeeDto>>(jsonData);
                return View(values);
            }
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterEmployee()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterEmployee(CreateEmployeeDto newEmployee)
        {
            EmployeeRegisterValidator validations = new EmployeeRegisterValidator();
            ValidationResult results = validations.Validate(newEmployee);
            if (results.IsValid)
            {
                newEmployee.DepartmanID = 1;
                var userRoles=User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value);
                var accessToken = HttpContext.Request.Cookies["AuthenticationToken"];
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var jsonData = JsonConvert.SerializeObject(newEmployee);
                StringContent jsonEmployee = new(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Auth/RegisterEmployee", jsonEmployee);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("AdminHomePage", "AdminHome");
                }
                else
                {
                    foreach (var item in results.Errors)
                    {
                        ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                    }
                }
            }
            return View();
        }
        public IActionResult WeeklySchedule()
        {
            return View();
        }
        //[HttpGet]
        //public async Task<IActionResult> EmployeeSchedule()
        //{
        //        var client = _httpClientFactory.CreateClient();
        //        var responseMessage = await client.GetAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/ScheduleUser"); // API_URL'nin doğru bir şekilde ayarlanması gerekiyor.
        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            var jsonData = await responseMessage.Content.ReadAsStringAsync();
        //            var events = JsonConvert.DeserializeObject<List<ScheduleEmployeeDto>>(jsonData);
        //            return View(events);
        //        }
        //        else
        //        {
        //            // API'den veri alınamazsa uygun bir hata mesajı döndürebilirsiniz.
        //            return View();
        //        }

        //}
        [HttpGet]
        public async Task<IActionResult> EmployeeSchedule(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/ScheduleUser/{id}"); 
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var events = JsonConvert.DeserializeObject<List<ScheduleEmployeeDto>>(jsonData);
                return View(events);
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeDetails(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Employee/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<ResultEmployeeDto>(jsonData);

                // Get the department details
                var departmentResponse = await client.GetAsync($"https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Department/{employee.DepartmentID}");
                if (departmentResponse.IsSuccessStatusCode)
                {
                    var departmentData = await departmentResponse.Content.ReadAsStringAsync();
                    var department = JsonConvert.DeserializeObject<ResultDepartmentDto>(departmentData);
                    ViewBag.DepartmentName = department.Name;
                }
                else
                {
                    employee.DepartmentName = "Not assigned";
                }

                return View(employee);
            }
            else
            {
                // Log the error message or handle it as needed
                ViewBag.ErrorMessage = "Failed to load employee details.";
            }
            return View(null); // Passing null to the view in case of an error
        }


    }


}

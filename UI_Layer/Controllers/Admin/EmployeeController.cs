using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using UI_Layer.Dtos.AnnouncementDto;
using UI_Layer.Dtos.EmployeeDto;
using UI_Layer.ValidationRules.Announcement;
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
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RegisterEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterEmployee(CreateEmployeeDto newEmployee)
        {
            EmployeeRegisterValidator validations = new EmployeeRegisterValidator();
            ValidationResult results = validations.Validate(newEmployee);
            if (results.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var jsonData = JsonConvert.SerializeObject(newEmployee);
                StringContent jsonEmployee = new(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("http://localhost:5144/api/Auth/Register", jsonEmployee);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Announcement");
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
    }
}

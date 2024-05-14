using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using UI_Layer.Dtos.EmployeeDto;
using UI_Layer.ValidationRules.Employee;

namespace UI_Layer.Controllers.Login
{
    public class LoginEmployeeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public LoginEmployeeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public IActionResult LoginEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginEmployee(LoginEmployeeDto loginEmployee)
        {
            EmployeeLoginValidator validations = new EmployeeLoginValidator();
            ValidationResult results = validations.Validate(loginEmployee);
            if (results.IsValid)
            {
                var client = _httpClientFactory.CreateClient();

                var jsonData = JsonConvert.SerializeObject(loginEmployee);
                StringContent jsonEmployee = new(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("http://localhost:27312/api/Auth/Login", jsonEmployee);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var tokenResponse = await responseMessage.Content.ReadAsStringAsync();
                    var tokenObject = JsonConvert.DeserializeObject<dynamic>(tokenResponse);
                    var token = tokenObject.Token;

                    // Tokenı local storage'a kaydet
                    if (token != null)
                    {
                        // Tarayıcıya çerez olarak eklenmesi gereken işlemler
                        HttpContext.Response.Cookies.Append("AuthenticationToken", token);
                    }
                    return RedirectToAction("Index", "EmployeeHome");
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
        public IActionResult LogoutEmployee()
        {
            HttpContext.Response.Cookies.Delete("AuthenticationToken");
            return RedirectToAction("Index", "Announcement");
        }
    }
}

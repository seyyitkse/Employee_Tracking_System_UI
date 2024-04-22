using FluentValidation.Results;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using UI_Layer.Dtos.AnnouncementDto;
using UI_Layer.Dtos.EmployeeDto;
using UI_Layer.Dtos.JwtDto;
using UI_Layer.Models;
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
        [HttpGet]
        public async Task <IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",accessToken);
            var responseMessage = await client.GetAsync("http://localhost:5144/api/Employee");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultEmployeeDto>>(jsonData);
                return View(values);
            }
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
        [HttpGet]
        public IActionResult LoginEmployee()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> LoginEmployee(LoginEmployeeDto loginEmployee)
        //{
        //    EmployeeLoginValidator validations = new EmployeeLoginValidator();
        //    ValidationResult results = validations.Validate(loginEmployee);
        //    if (results.IsValid)
        //    {
        //        var client = _httpClientFactory.CreateClient();

        //        var jsonData = JsonConvert.SerializeObject(loginEmployee);
        //        StringContent jsonEmployee = new(jsonData, Encoding.UTF8, "application/json");
        //        var responseMessage = await client.PostAsync("http://localhost:5144/api/Auth/Login", jsonEmployee);

        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            return RedirectToAction("Index", "Announcement");
        //        }
        //        else
        //        {
        //            foreach (var item in results.Errors)
        //            {
        //                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
        //            }
        //        }
        //    }
        //    return View();
        //}
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
                var responseMessage = await client.PostAsync("http://localhost:5144/api/Auth/Login", jsonEmployee);

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

using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using UI_Layer.Dtos.AdminDto;
using UI_Layer.ValidationRules.Admin;

namespace UI_Layer.Controllers.Login
{
    public class LoginAdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public LoginAdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public IActionResult LoginAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAdmin(LoginAdminDto loginAdmin)
        {
            AdminLoginValidator validations = new AdminLoginValidator();
            ValidationResult results = validations.Validate(loginAdmin);
            if (results.IsValid)
            {
                var client = _httpClientFactory.CreateClient();

                var jsonData = JsonConvert.SerializeObject(loginAdmin);
                StringContent jsonAdmin = new(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Auth/Login", jsonAdmin);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var tokenResponse = await responseMessage.Content.ReadAsStringAsync();
                    var tokenObject = JsonConvert.DeserializeObject<dynamic>(tokenResponse);
                    var token = tokenObject.Token;

                    // Tokenı local storage'a kaydet
                    if (token != null)
                    {
                        // Çerez olarak eklenmesi gereken işlemler
                        // HttpContext.Response.Cookies.Append("AuthenticationToken", token);
                        // Yukarıdaki satırı aşağıdaki ile değiştiriyoruz:
                        Response.Cookies.Append("AuthenticationToken", token, new CookieOptions
                        {
                            HttpOnly = true, // Tarayıcı tarafından erişilemez
                            Secure = true,   // HTTPS üzerinden iletişimde kullanılabilir
                            SameSite = SameSiteMode.None // Çerezin farklı origin'lere de gönderilmesine izin verilir
                        });
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
        public IActionResult LogoutAdmin()
        {
            HttpContext.Response.Cookies.Delete("AuthenticationToken");
            return RedirectToAction("Index", "Announcement");
        }
        //[HttpPost]
        //public async Task<IActionResult> LoginAdmin(LoginAdminDto loginAdmin)
        //{
        //    AdminLoginValidator validations = new AdminLoginValidator();
        //    ValidationResult results = validations.Validate(loginAdmin);
        //    if (results.IsValid)
        //    {
        //        var client = _httpClientFactory.CreateClient();

        //        var jsonData = JsonConvert.SerializeObject(loginAdmin);
        //        StringContent jsonAdmin = new(jsonData, Encoding.UTF8, "application/json");
        //        var responseMessage = await client.PostAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Auth/Login", jsonAdmin);

        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            var tokenResponse = await responseMessage.Content.ReadAsStringAsync();
        //            var tokenObject = JsonConvert.DeserializeObject<dynamic>(tokenResponse);
        //            var token = tokenObject.Token;

        //            // Tokenı local storage'a kaydet
        //            if (token != null)
        //            {
        //                // Tarayıcıya çerez olarak eklenmesi gereken işlemler
        //                HttpContext.Response.Cookies.Append("AuthenticationToken", token);
        //            }
        //            return RedirectToAction("Index", "EmployeeHome");
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
    }
}

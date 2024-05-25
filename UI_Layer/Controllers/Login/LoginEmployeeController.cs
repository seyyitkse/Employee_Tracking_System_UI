using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UI_Layer.Authorization;
using UI_Layer.Dtos.EmployeeDto;

namespace UI_Layer.Controllers.Login
{
    [AllowAnonymous]
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
        public async Task<IActionResult> LoginEmployee(LoginEmployeeDto model)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:27312/api/Auth/Login", model);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Error");
            }

            // Read and log the raw JSON response
            var responseDataJson = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"API Response: {responseDataJson}");

            // Ensure the response data is valid JSON
            if (string.IsNullOrWhiteSpace(responseDataJson))
            {
                System.Diagnostics.Debug.WriteLine("Error: Response data is null or empty.");
                return RedirectToAction("Error");
            }

            try
            {
                // Deserialize the JSON response
                var responseData = JsonConvert.DeserializeObject<AuthResponse>(responseDataJson);
                var tokenValue = responseData?.Token;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(tokenValue) as JwtSecurityToken;
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                if (string.IsNullOrEmpty(tokenValue))
                {
                    throw new Exception("Token not found in the response.");
                }
                string userRole = roleClaim.Value;
                // Store the token in a cookie
                Response.Cookies.Append("AuthenticationToken", tokenValue, new CookieOptions
                {
                    //httponly açılırsa javascript tarafında kullanılamaz!!!
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });

                // Use role information to redirect user
                if (userRole == "Admin")
                {
                    return RedirectToAction("AdminHomePage", "AdminHome");
                }
                else if (userRole == "Employee")
                {
                    // Redirect non-admin users to a different page
                    return RedirectToAction("Index", "EmployeeHome");
                }
                else
                {
                    // Log the error
                    System.Diagnostics.Debug.WriteLine("Error: Invalid role in the token.");
                    return RedirectToAction("Error");
                }
            }
            catch (JsonException ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"JSON Deserialization Error: {ex.Message}");
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                return RedirectToAction("Error");
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> LoginEmployee(LoginEmployeeDto loginEmployee)
        //{

        //        var client = _httpClientFactory.CreateClient();

        //        var jsonData = JsonConvert.SerializeObject(loginEmployee);
        //        StringContent jsonEmployee = new(jsonData, Encoding.UTF8, "application/json");
        //        var responseMessage = await client.PostAsync("http://localhost:27312/api/Auth/Login", jsonEmployee);

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
        //            return RedirectToAction("EmployeeHomePage", "EmployeeHome");
        //        }
        //        return View();
        //}

        public IActionResult LogoutEmployee()
        {
            HttpContext.Response.Cookies.Delete("AuthenticationToken");
            return RedirectToAction("Index", "Announcement");
        }
    }
}

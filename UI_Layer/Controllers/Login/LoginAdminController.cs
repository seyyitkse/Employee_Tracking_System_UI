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
        public async Task<IActionResult> LoginAdmin(LoginEmployeeDto model)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Auth/Login", model);

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
                    HttpOnly = true,
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
                    Response.Cookies.Delete("AuthenticationToken");
                    // Redirect non-admin users to a different page
                    return RedirectToAction("AccessDenied", "ErrorPage");
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

        public IActionResult LogoutAdmin()
        {
            HttpContext.Response.Cookies.Delete("AuthenticationToken");
            return RedirectToAction("HomePage", "HomeScreen");
        }

    }
}

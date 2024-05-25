using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using UI_Layer.Dtos.AdminDto;
using UI_Layer.ValidationRules.Admin;
using System.IdentityModel.Tokens.Jwt;
using UI_Layer.Dtos.EmployeeDto;
using UI_Layer.Authorization;
using Newtonsoft.Json.Linq;
using Humanizer;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Login(LoginEmployeeDto model)
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

        public IActionResult LogoutAdmin()
        {
            HttpContext.Response.Cookies.Delete("AuthenticationToken");
            return RedirectToAction("HomePage", "HomeScreen");
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
        //        var responseMessage = await client.PostAsync("http://localhost:27312/api/Auth/Login", jsonAdmin);

        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            var tokenResponse = await responseMessage.Content.ReadAsStringAsync();
        //            var tokenObject = JsonConvert.DeserializeObject<dynamic>(tokenResponse);
        //            var token = tokenObject.Token.ToString();

        //            if (!string.IsNullOrEmpty(token))
        //            {
        //                // Decode the token
        //                var tokenHandler = new JwtSecurityTokenHandler();
        //                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        //                // Check if the token is valid and not expired
        //                if (jwtToken != null && jwtToken.ValidTo > DateTime.UtcNow)
        //                {
        //                    // Access role information from the token payload
        //                    var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        //                    if (roleClaim != null)
        //                    {
        //                        string userRole = roleClaim.Value;

        //                        // Use role information as needed
        //                        if (userRole == "Admin")
        //                        {
        //                            HttpContext.Response.Cookies.Append("AuthenticationToken", token, new CookieOptions
        //                            {
        //                                HttpOnly = true,
        //                                Secure = true,
        //                                SameSite = SameSiteMode.Strict
        //                            });

        //                            return RedirectToAction("AdminHomePage", "AdminHome");
        //                        }
        //                        else
        //                        {
        //                            ModelState.AddModelError(string.Empty, "Unauthorized access. You are not an admin.");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ModelState.AddModelError(string.Empty, "Role information not found in the token.");
        //                    }
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(string.Empty, "Invalid or expired token received from the API.");
        //                }
        //            }
        //            else
        //            {
        //                ModelState.AddModelError(string.Empty, "Token not received from the API.");
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //        }
        //    }
        //    return View(loginAdmin);
        //}


    }
}

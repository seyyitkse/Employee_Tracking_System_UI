using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using UI_Layer.Dtos.EmployeeDto;

namespace UI_Layer.Controllers.Admin
{
    public class AdminHomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminHomeController(IHttpClientFactory httpClient)
        {
            _httpClientFactory = httpClient;
        }

        [Authorize(Policy = "AdminPolicy")]
        public async Task< IActionResult> AdminHomePage()
        {
            var token = Request.Cookies["AuthenticationToken"];
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token not found in cookies.");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username");
            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Name");
            var client = _httpClientFactory.CreateClient();
            ViewBag.Username = userNameClaim.Value;
            ViewBag.Name = nameClaim.Value;
          
            return View();
            
        }
        [HttpGet]
        public IActionResult LogoutAdmin()
        {
                var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Path = "/", // The path of the cookie. Adjust this value if your cookie was created with a different path.
                    Domain = null, // The domain of the cookie. Adjust this value if your cookie was created with a specific domain.
                    Secure = false, // Set this to true if your cookie was created with the Secure flag.
                    HttpOnly = false, // Set this to true if your cookie was created with the HttpOnly flag.
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax, // Adjust this value if your cookie was created with a different SameSite setting.
                    Expires = DateTime.Now.AddDays(-100) // Set the cookie to expire in the past.
                };

                Response.Cookies.Append("AuthenticationToken", "", cookieOptions); // Overwrite the cookie with an expired cookie

                return RedirectToAction("HomePage", "HomeScreen");
        }
    }
}

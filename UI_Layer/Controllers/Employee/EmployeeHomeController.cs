using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UI_Layer.Dtos.AnnouncementDto;
using UI_Layer.Dtos.EmployeeDto;
using UI_Layer.Dtos.NotificationsDto;

namespace UI_Layer.Controllers.Employee
{
    [Authorize(Policy = "EmployeePolicy")]
    public class EmployeeHomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeeHomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["AuthenticationToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "LoginAdmin");
            }

            // Token'ı ayrıştır
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null)
            {
                return RedirectToAction("Login", "LoginAdmin");
            }

            // ID'yi token içindeki claim'lerden al
            var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserID");
            if (idClaim == null)
            {
                return RedirectToAction("Login", "LoginAdmin");
            }

            int id = int.Parse(idClaim.Value);

            // API isteği yap
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/ScheduleUser/${id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var info = JsonConvert.DeserializeObject<ResultEmployeeDto>(jsonData);
                return View(info);
            }
            else
            {
                return View();
            }
        }
        public IActionResult HomeEmployee()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Announcements()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Announcement");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAnnouncementDto>>(jsonData);
                var lastFiveAnnouncements = values.OrderByDescending(a => a.Date).Take(5).ToList(); 
                return View(lastFiveAnnouncements);
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeInfo(int id)
        {
            var token = Request.Cookies["AuthenticationToken"];
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token not found in cookies.");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username");
            var departmentNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Department");
            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Name");
            var client = _httpClientFactory.CreateClient();
            ViewBag.Username = userNameClaim.Value;
            ViewBag.DepartmentName=departmentNameClaim.Value;
            ViewBag.Name = nameClaim.Value;
            var responseMessage = await client.GetAsync($"https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Employee/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var info = JsonConvert.DeserializeObject<List<ResultEmployeeDto>>(jsonData);
                return View(info);
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var token = Request.Cookies["AuthenticationToken"];
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token not found in cookies.");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserID");
            if (userIdClaim == null)
            {
                throw new Exception("UserID claim not found in token.");
            }

            var userId = userIdClaim.Value;

            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Alert/getAlerts/{userId}");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var notifications = JsonConvert.DeserializeObject<List<ResultNotificationDto>>(jsonData);
                return View(notifications);
            }
            else
            {
                throw new Exception($"Error fetching notifications: {responseMessage.StatusCode}");
            }
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var token = Request.Cookies["AuthenticationToken"];
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token not found in cookies.");
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            if (ModelState.IsValid)
            {
       
                var userNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username");
                // Get the user's email from the User property
                model.Email = userNameClaim.Value;

                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("http://localhost:27312/api/Auth/ChangePassword", content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["NotificationMessage"] = "Password changed successfully.";
                    TempData["NewPassword"] = model.NewPassword;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Current password is incorrect.");
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult LogoutEmployee()
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
        public static string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assuming the user ID is stored in the "sub" claim
            var userId = jwtToken.Claims.First(claim => claim.Type == "sub").Value;

            return userId;
        }
    }
}

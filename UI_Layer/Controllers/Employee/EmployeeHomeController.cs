using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UI_Layer.Dtos.AnnouncementDto;
using UI_Layer.Dtos.EmployeeDto;

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
            var client = _httpClientFactory.CreateClient();
            ViewBag.Username = userNameClaim.Value;
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
        public IActionResult Notifications()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = client.GetAsync("https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/Notification");
            return View();
        }
        [HttpGet]
        public IActionResult LogoutEmployee()
        {
            // Remove the authentication token
            HttpContext.Response.Cookies.Delete("AuthenticationToken");
            // Redirect to the login page
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

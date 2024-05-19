using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UI_Layer.Dtos.AnnouncementDto;
using UI_Layer.Dtos.EmployeeDto;

namespace UI_Layer.Controllers.Employee
{
    public class EmployeeHomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeeHomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://trackingprojectwebappservice20240505190044.azurewebsites.net/api/ScheduleUser/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var events = JsonConvert.DeserializeObject<List<ScheduleEmployeeDto>>(jsonData);
                return View(events);
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
            var responseMessage = await client.GetAsync("http://localhost:27312/api/Announcement");
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
          var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"http://localhost:27312/api/Employee/{id}");
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
        public IActionResult LogoutEmployee()
        {
            // Remove the authentication token
            HttpContext.Response.Cookies.Delete("jwt");
            // Redirect to the login page
            return RedirectToAction("LoginEmployee", "LoginEmployee");
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

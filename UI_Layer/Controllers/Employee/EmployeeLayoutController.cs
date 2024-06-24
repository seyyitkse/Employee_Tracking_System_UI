using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using UI_Layer.Dtos.NotificationsDto;

namespace UI_Layer.Controllers.Employee
{
    public class EmployeeLayoutController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeeLayoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult EmployeeLayout()
        {
            return View();
        }
        public PartialViewResult PartialHead()
        {
            return PartialView();
        }
        public PartialViewResult PartialSidebar()
        {
            return PartialView();
        }
        public async Task<PartialViewResult> PartialHeader()
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
                if (notifications != null && notifications.Any())
                {
                    var lastNotifications = notifications.TakeLast(3).ToList();
                    return PartialView(lastNotifications);
                }
                else
                {
                    // Handle the case when there are no notifications
                    // For example, you can return an empty list:
                    return PartialView(new List<ResultNotificationDto>());
                }
            }
            else
            {
                throw new Exception($"Error fetching notifications: {responseMessage.StatusCode}");
            }
        }
        public PartialViewResult PartialScript()
        {
            return PartialView();
        }
    }
}

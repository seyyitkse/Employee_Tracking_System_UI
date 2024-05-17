using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
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
    }
}

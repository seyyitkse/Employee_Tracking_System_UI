using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using UI_Layer.Dtos.AnnouncementDto;
using UI_Layer.Models;
using UI_Layer.ValidationRules.Announcement;

namespace UI_Layer.Controllers.Admin
{
    [Authorize]
    public class AnnouncementController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AnnouncementController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWTToken"));
            var responseMessage = await client.GetAsync("http://localhost:5144/api/Announcement");
            if (responseMessage.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("JWTToken", responseObject.Message);
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAnnouncementDto>>(jsonData);
                return View(values);
            }
            return View();
        }
        [HttpGet]
        public IActionResult AddAnnouncement()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAnnouncement(CreateAnnouncementDto newAnnouncement)
        {
            CreateAnnouncementValidator validations=new CreateAnnouncementValidator();
            ValidationResult results=validations.Validate(newAnnouncement);
            if (results.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWTToken"));
                var jsonData = JsonConvert.SerializeObject(newAnnouncement);
                StringContent jsonAnnouncement = new(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("http://localhost:5144/api/Announcement", jsonAnnouncement);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseBody = await responseMessage.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<EmployeeManagerResponse>(responseBody);

                    if (responseObject.IsSuccess)
                    {
                        // JWT'yi yerel depolamada sakla
                        HttpContext.Session.SetString("JWTToken", responseObject.Message);

                        return RedirectToAction("Index", "Announcement");
                    }
                    else
                    {
                        // Kullanıcı girişi başarısız oldu
                        ModelState.AddModelError(string.Empty, "Invalid email or password");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please try again later.");
                    return View();
                }
            }
            return View();
        }
        [Route("announcementDetails")]
        [HttpGet("{id}")]
        public async Task<IActionResult> AnnouncementDetails(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"http://localhost:5144/api/Announcement/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<ResultAnnouncementDto>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}

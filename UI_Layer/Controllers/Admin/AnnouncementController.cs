using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using UI_Layer.Dtos.AnnouncementDto;
using UI_Layer.ValidationRules.Announcement;

namespace UI_Layer.Controllers.Admin
{
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
            var responseMessage = await client.GetAsync("http://localhost:5144/api/Announcement");
            if (responseMessage.IsSuccessStatusCode)
            {
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
                var jsonData = JsonConvert.SerializeObject(newAnnouncement);
                StringContent jsonAnnouncement = new(jsonData, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync("http://localhost:5144/api/Announcement", jsonAnnouncement);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Announcement");
                }
                else
                {
                    foreach (var item in results.Errors)
                    {
                        ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                    }
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

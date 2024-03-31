using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UI_Layer.Dtos;
using UI_Layer.Dtos.AnnouncementDto;
namespace UI_Layer.Controllers
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
            var client=_httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("http://localhost:5144/api/Announcement");
            if (responseMessage.IsSuccessStatusCode) 
            {
                var jsonData=await responseMessage.Content.ReadAsStringAsync();
                var values=JsonConvert.DeserializeObject<List<ResultAnnouncementDto>>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}

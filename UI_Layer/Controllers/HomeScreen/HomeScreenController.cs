using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Controllers.HomeScreen
{
    [AllowAnonymous]
    public class HomeScreenController : Controller
    {
        [HttpGet]
        public IActionResult HomePage()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}

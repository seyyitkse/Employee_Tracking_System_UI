using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Controllers.HomeScreen
{
    public class HomeScreenController : Controller
    {
        public IActionResult HomePage()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}

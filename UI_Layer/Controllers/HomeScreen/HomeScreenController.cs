using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Controllers.HomeScreen
{
    [AllowAnonymous]
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

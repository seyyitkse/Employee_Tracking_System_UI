using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Controllers.Admin
{
    public class AdminHomeController : Controller
    {
        public IActionResult AdminHomePage()
        {
            return View();
        }
    }
}

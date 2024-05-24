using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Controllers.Admin
{
    public class AdminHomeController : Controller
    {
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AdminHomePage()
        {
            return View();
        }
        [HttpGet]
        public IActionResult LogoutEmployee()
        {
            HttpContext.Response.Cookies.Delete("AuthenticationToken");
            return RedirectToAction("HomePage", "HomeScreen");
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Controllers.Admin
{
    public class AdminHomeController : Controller
    {
        public IActionResult AdminHomePage()
        {
            return View();
        }
        [HttpGet]
        public IActionResult LogoutEmployee()
        {
            // Remove the authentication token
            HttpContext.Response.Cookies.Delete("jwt");
            // Redirect to the login page
            return RedirectToAction("LoginAdmin", "LoginAdmin");
        }
    }
}

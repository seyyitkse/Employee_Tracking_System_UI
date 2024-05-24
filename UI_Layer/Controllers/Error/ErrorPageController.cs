using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Controllers.Error
{
    public class ErrorPageController : Controller
    {
        public IActionResult ErrorPage(int statusCode)
        {
            ViewBag.StatusCode = statusCode;
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

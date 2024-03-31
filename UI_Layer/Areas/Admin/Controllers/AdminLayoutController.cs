using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Areas.Admin.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult AdminLayout()
        {
            return View();
        }
        public PartialViewResult PartialHead()
        {
            return PartialView();
        }
        public PartialViewResult PartialSidebar()
        {
            return PartialView();
        }
        public PartialViewResult PartialHeader()
        {
            return PartialView();
        }
    }
}

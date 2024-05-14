using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Controllers.Employee
{
    public class EmployeeHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

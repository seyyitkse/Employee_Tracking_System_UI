﻿using Microsoft.AspNetCore.Mvc;

namespace UI_Layer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;

namespace BakeryManager.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

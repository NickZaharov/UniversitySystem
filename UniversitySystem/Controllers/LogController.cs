using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UniversitySystem.Controllers
{
    public class LogController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Subject = 0;
            return View();
        }
    }
}
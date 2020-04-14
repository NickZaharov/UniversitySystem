using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Models;

namespace UniversitySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly UniversityDbContext db;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(UniversityDbContext context, UserManager<IdentityUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

    public IActionResult Index()
    {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (db.UserRoles.FirstOrDefault(p => p.UserId == UserId) ==null)
            {
                ViewBag.authorize = 0;
            }
        return View();
    }

    public IActionResult MyCourses()
    {
        foreach (var item in db.Faculties.ToList())
        {
            ViewData["Message"] = ViewData["Message"] + "  " + item.Name;
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

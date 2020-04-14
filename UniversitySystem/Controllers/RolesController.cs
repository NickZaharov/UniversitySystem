using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using UniversitySystem.Data;

namespace UniversitySystem.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        readonly UniversityDbContext db;
        public RolesController(UniversityDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            ViewBag.Users = db.Users.ToList();
            return View();
        }

        public IActionResult Edit(string id)
        {
            ViewBag.User = db.Users.Where(p=>p.Id == id).ToList();
            ViewBag.Roles = db.Roles.ToList();
            ViewBag.Id = id;
            ViewBag.Check = db.UserRoles.Where(p => p.UserId == id).ToList();
            ViewBag.i = 0;
            return View();
        }

        [HttpPost]
        public IActionResult EditRoles(string[] RoleId, string id)
        {
            foreach (var item in db.UserRoles.Where(p => p.UserId == id))
            {
                db.UserRoles.Remove(item);
            }
            foreach (var item in RoleId)
            {
                db.UserRoles.Add(new IdentityUserRole<string> { UserId = id, RoleId = item });
            }
        db.SaveChanges();
        return Redirect("/Roles/Index");    
        }
    }
}
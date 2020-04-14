using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Calabonga.Xml.Exports;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace UniversitySystem.Controllers
{
    public class Rate
    {

        public int RatingId { get; set; }
        public int StudentId { get; set; }
        public string Value { get; set; }
        public int LogId { get; set; }
    }

    public class Stud
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class CourseController : Controller
    {
        readonly UniversityDbContext db;
        private readonly UserManager<IdentityUser> _userManager;
        public CourseController(UniversityDbContext context, UserManager<IdentityUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewBag.authorized = 0;
            ViewBag.groups = db.Groups.ToList();
            ViewBag.faculties = db.Faculties.ToList();
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = db.Teachers.FirstOrDefault(s => s.UserId == UserId);
            var student = db.Students.FirstOrDefault(s => s.UserId == UserId);
            if (teacher == null && student == null)
            {
                ViewBag.Count = 0;
                return View();
            }
            if(student == null)
            {
                ViewBag.authorized = 1;
                var courses = db.Courses
                    .Include(s => s.Subject)
                    .Include(s => s.Teacher)
                    .Include(s => s.Faculty)
                    .Include(s => s.Group)
                    .Where(m => m.TeacherId == teacher.TeacherId);
                return View(courses);
            }
            if (teacher == null)
            {
                ViewBag.authorized = 2;
                var courseMembers = db.CourseMembers.Where(p => p.StudentId == student.StudentId);
                var courses = db.Courses
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .Include(s => s.Faculty)
                .Include(s => s.Group)
                .Where(e => courseMembers.Any(x => x.CourseId == e.CourseId));
                return View(courses);
            }
            if (teacher == null && student == null)
            {
                ViewBag.authorized = 0;
                return View();
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(string group)
        {
            ViewBag.authorized = 1;
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null)
            {
                ViewBag.authorized = 0;
                return View();
            }
            var teacher = db.Teachers.FirstOrDefault(s => s.UserId == UserId);
            var student = db.Students.FirstOrDefault(s => s.UserId == UserId);
            if (teacher == null && student == null)
            {
                ViewBag.Count = 0;
                return View();
            }
            if (student == null)
            {
                ViewBag.authorized = 1;
                var courses = db.Courses
                    .Include(s => s.Subject)
                    .Include(s => s.Teacher)
                    .Include(s => s.Faculty)
                    .Include(s => s.Group)
                    .Where(m => m.TeacherId == teacher.TeacherId);
                ViewBag.groups = db.Groups.ToList();
                ViewBag.groups = db.Groups;
                if (!string.IsNullOrEmpty(group))
                {
                    courses = courses.Where(p => p.Group.Name == group);
                }
                return View(courses);
            }
            if (teacher == null)
            {
                ViewBag.groups = db.Groups.ToList();
                ViewBag.authorized = 2;
                var courseMembers = db.CourseMembers.Where(p => p.StudentId == student.StudentId);
                var courses = db.Courses
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .Include(s => s.Faculty)
                .Include(s => s.Group)
                .Where(e => courseMembers.Any(x => x.CourseId == e.CourseId));
                if (!string.IsNullOrEmpty(group))
                {
                    courses = courses.
                     Include(s => s.Subject)
                    .Include(s => s.Teacher)
                    .Include(s => s.Faculty)
                    .Include(s => s.Group).Where(p => p.Group.Name == group);
                    return View(courses);
                }
                else
                {
                    return View(courses);
                }
            }
            return View();
        }

        public IActionResult Create(string str)
        {
            if(str == "warning")
            {
                ViewBag.warning = 1;
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.TeacherId = db.Teachers.Where(p => p.UserId == userId).FirstOrDefault().TeacherId;
            ViewData["SubjectId"] = new SelectList(db.Subjects, "Subjectid", "Name");
            ViewData["FacultyId"] = new SelectList(db.Faculties, "FacultyId", "Name");
            ViewData["GroupId"] = new SelectList(db.Groups, "GroupId", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("SubjectId,GroupId,FacultyId")] Course course, int id)
        {
            if (db.Courses.FirstOrDefault(p => p.TeacherId == course.TeacherId && p.SubjectId == course.SubjectId && p.GroupId == course.GroupId) != null)
            {
                RedirectToAction("Create", "Course", new { str = "warning" });
            }
            course.TeacherId = id;
            db.Courses.Add(course);
            db.SaveChanges();
            var courseAfter = db.Courses.FirstOrDefault(p => p.TeacherId == course.TeacherId && p.SubjectId == course.SubjectId && p.GroupId == course.GroupId);
            var members = db.Students.Where(p => p.GroupId == course.GroupId).ToList();
            foreach (var item in members)
            {
                db.CourseMembers.Add(new CourseMember { CourseId = courseAfter.CourseId, StudentId = item.StudentId });
            }
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = db.Teachers.FirstOrDefault(s => s.UserId == UserId);
            if (teacher == null)
            {
                ViewBag.authorized = 0;
            }
            else
            {
                ViewBag.authorized = 1;
            }
                ViewBag.Id = id;
            var logs = db.Logs.Where(p => p.CourseId == id);
            ViewBag.ratings = db.Ratings.Join(logs.DefaultIfEmpty(), p => p.LogId, c => c.LogId,
                (p, c) => new Rate
                {
                    RatingId = p.RatingId,
                    StudentId = p.StudentId,
                    Value = p.Value,
                    LogId = p.LogId
               }
               ).ToList();
            ViewBag.logs = logs.ToList();
            var courseMembers = db.CourseMembers.Where(p => p.CourseId == id);
            ViewBag.students = db.Students.Join(courseMembers.DefaultIfEmpty(), p => p.StudentId, c => c.StudentId,
                (p, c) => new Stud
                {
                    StudentId = p.StudentId,
                    Name = p.Name,
                    Surname = p.Surname
                }
               ).ToList();
            ViewBag.course = db.Courses
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .Include(s => s.Faculty)  
                .Include(s => s.Group)
                .FirstOrDefault(m => m.CourseId == id);
            return View();
        }

        public IActionResult CreateEvent(string name, int id)
        {
            int courseId = id;
            db.Logs.Add(new Log { CreateDate = DateTime.Now, lastModified = DateTime.Now, CourseId = id, Title = name });
            var courseMembers = db.CourseMembers.Where(p => p.CourseId == id);
            var students = db.Students.Join(courseMembers.DefaultIfEmpty(), p => p.StudentId, c => c.StudentId,
                (p, c) => new Stud
                {
                    StudentId = p.StudentId,
                    Name = p.Name,
                    Surname = p.Surname
                }
               ).ToList();
            db.SaveChanges();
            var log = db.Logs.FirstOrDefault(p=>p.CourseId == id && p.Title == name);
            foreach(var item in students)
            {
                db.Ratings.Add(new Rating { LogId = log.LogId, StudentId = item.StudentId});
            }
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = courseId});
        }

        public IActionResult EditRating(int id, List<Rating> rating)
        {
            int courseId = id;
            ViewBag.course = db.Courses
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .Include(s => s.Faculty)
                .Include(s => s.Group)
                .FirstOrDefault(m => m.CourseId == id);
            foreach (var item in rating)
            {
                db.Update(item);
            }
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = courseId });
        }

        public IActionResult DelLog(int id, int LogId)
        {
            var log = db.Logs.Find(LogId);
            db.Logs.Remove(log);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id });
        }

        public IActionResult Members(int id)
        {
            ViewBag.course = db.Courses
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .Include(s => s.Faculty)
                .Include(s => s.Group)
                .FirstOrDefault(m => m.CourseId == id);
            ViewBag.members = db.CourseMembers
               .Include(s => s.Student)
               .Include(s => s.Course)
               .Where(m => m.CourseId == id).ToList();
            return View();
        }

        public void genExcel(int id)
        {
            var logs = db.Logs.Where(p => p.CourseId == id);
            ViewBag.ratings = db.Ratings.Join(logs.DefaultIfEmpty(), p => p.LogId, c => c.LogId,
                (p, c) => new Rate
                {
                    RatingId = p.RatingId,
                    StudentId = p.StudentId,
                    Value = p.Value,
                    LogId = p.LogId
                }
               ).ToList();
            ViewBag.logs = logs.ToList();
            var courseMembers = db.CourseMembers.Where(p => p.CourseId == id);
            ViewBag.students = db.Students.Join(courseMembers.DefaultIfEmpty(), p => p.StudentId, c => c.StudentId,
                (p, c) => new Stud
                {
                    StudentId = p.StudentId,
                    Name = p.Name,
                    Surname = p.Surname
                }
               ).ToList();

            Workbook wb = new Workbook();
            Worksheet ws = new Worksheet("ws");
            ws.AddCell(0, 0, "----------");
            int i = 1;
            foreach (var item in ViewBag.logs)
            {
                ws.AddCell(0, i, item.Title);
                i++;
            }
            i = 1;
            foreach (var item in ViewBag.students)
            {
                int y = 1;
                ws.AddCell(i, 0, item.Name+" "+item.Surname);
                foreach(var item2 in ViewBag.ratings)
                {
                    if (item2.StudentId == item.StudentId)
                    {
                        if (item2.Value == null)
                        {
                            ws.AddCell(i, y, "");
                        }
                        else
                        {
                            ws.AddCell(i, y, @item2.Value);
                        }
                        y++;
                    }
                }
                i++;
            }
            wb.AddWorksheet(ws);
                
            string filename = "Rating.xls";
            string workstring = wb.ExportToXML();
            HttpContext.Response.Headers.Add("content-disposition",string.Format("attachment; filename={0}", filename));
            HttpContext.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Response.WriteAsync(workstring);
        }

        public IActionResult DelCourse(int id)
        {
            var course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult PanelPartial(int id)
        {
            ViewBag.course = db.Courses

                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .Include(s => s.Faculty)
                .Include(s => s.Group)
                .FirstOrDefault(m => m.CourseId == id);
            return View();
        }
    }
}




/*<input asp-for="Value" value="@item2.Value" class="form-control" />
                                <input asp-for="RatingId" value="@item2.RatingId" style="display:none" />*/
/*<div class="dropdown js-dropdown" id="dropdown-styles" aria-hidden="false" style="height: 695px; overflow-y: scroll;">
    <ul class="dropdown-list">
        <li class="inputwrapper">
            <input name = "dropdown-styles" class="custom-input" id="dropdown-styles-v-образныйвырез" type="checkbox" value="v-%D0%BE%D0%B1%D1%80%D0%B0%D0%B7%D0%BD%D1%8B%D0%B9+%D0%B2%D1%8B%D1%80%D0%B5%D0%B7" data-name="styles">
            <label class="label" for="dropdown-styles-v-образныйвырез">

                <span class="text">v-образный вырез</span>
                <span class="item-count">4</span>
            </label>
        </li>
     </ul>
</div>*/

/*<select style="visibility:hidden" name="group" class="list-group-item row-right option-select">
                        <option value=""></option>
                        @foreach (var item in ViewBag.faculties)
                        {
                            <option id="option" onchange="OptionChanged()" value="@item.Name">@item.Name</option>
                        }
                    </select>*/

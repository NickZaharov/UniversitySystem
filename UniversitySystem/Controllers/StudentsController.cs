using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace UniversitySystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UniversityDbContext _context;

        public StudentsController(UniversityDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var universityDbContext = _context.Set<Student>().Include(s => s.Faculty).Include(s => s.Group);
            return View(await universityDbContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Set<Student>()
                .Include(s => s.Faculty)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "Name");
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,Name,Surname,SecondName,GroupId,FacultyId,UserId")] Student student)
        {
            if (ModelState.IsValid)
            {
                if (student.UserId != null)
                {
                    _context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string> { UserId = student.UserId, RoleId = "1" });
                    _context.SaveChanges();
                }
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "Name", student.FacultyId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name", student.GroupId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Set<Student>().FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "Name", student.FacultyId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name", student.GroupId);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,Name,Surname,SecondName,GroupId,FacultyId, UserId")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (student.UserId != null)
                    {
                        _context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string> { UserId = student.UserId, RoleId = "1" });
                        _context.SaveChanges();
                    }
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "Name", student.FacultyId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "GroupId", "Name", student.GroupId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Set<Student>()
                .Include(s => s.Faculty)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CourseMembers.Any(c => c.StudentId == id) != false){
                CourseMember courseMember = _context.Set<CourseMember>().Where(c=>c.StudentId==id).FirstOrDefault();
                _context.Set<CourseMember>().Remove(courseMember);
            }
            if (_context.Ratings.Any(c => c.StudentId == id) != false)
            {
                var rating = _context.Set<Rating>().Where(c => c.StudentId == id).FirstOrDefault();
                _context.Set<Rating>().Remove(rating);
            }
            await _context.SaveChangesAsync();
            var student = await _context.Set<Student>().FindAsync(id);
            _context.Set<Student>().Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));     
        }

        private bool StudentExists(int id)
        {
            return _context.Set<Student>().Any(e => e.StudentId == id);
        }
    }
}

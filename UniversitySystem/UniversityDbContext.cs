using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UniversitySystem.Controllers
{
    public class UniversityDbContext : IdentityDbContext
    {
        public DbSet<Log> Logs { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseMember> CourseMembers { get; set; }

        public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*modelBuilder.Entity<CourseMember>()
            .HasKey(p => new
            {
                p.CourseId,
                p.StudentId
            });*/
        }
    }
}

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace UniversitySystem
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string SecondName { get; set; }
        public int GroupId { get; set; }
        public int FacultyId { get; set; }
        public string UserId { get; set; }

        public List<CourseMember> CourseMembers { get; set; }
        public IdentityUser User { get; set; }
        public Faculty Faculty { get; set; }
        public Group Group { get; set; }
    }
}

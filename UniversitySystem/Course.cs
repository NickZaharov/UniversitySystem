using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversitySystem
{
    public class Course
    {
        public int CourseId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public int FacultyId { get; set; }
        public int GroupId { get; set; }

        public List<Log> Logs { get; set; }
        public List<CourseMember> CourseMembers { get; set; }
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }
        public Group Group { get; set; }
        public Faculty Faculty { get; set; }
    }
}

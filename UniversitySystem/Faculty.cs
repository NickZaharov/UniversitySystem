using System;
using System.Collections.Generic;
using System.Text;

namespace UniversitySystem
{
    public class Faculty
    {
        public int FacultyId { get; set; }
        public string Name { get; set; }

        public List<Student> Students { get; set; }
    }
}

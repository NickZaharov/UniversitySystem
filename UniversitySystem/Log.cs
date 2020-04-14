using System;
using System.Collections.Generic;
using System.Text;

namespace UniversitySystem
{
    public class Log
    {
        public int LogId { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; } 
        public DateTime lastModified { get; set; }
        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}

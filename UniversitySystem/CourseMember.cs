using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UniversitySystem
{
    public class CourseMember
    {
        public int CourseMemberId { get; set; }
        //[Key ,Column(Order = 1)]
        public int CourseId { get; set; }
       
        //[Key ,Column(Order = 2)]
        public int StudentId { get; set; }
        
        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}

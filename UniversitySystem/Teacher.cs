using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace UniversitySystem
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string SecondName { get; set; }
        public string Post { get; set; }
        public string UserId { get; set; }

        public IdentityUser User { get; set; }
    }
}

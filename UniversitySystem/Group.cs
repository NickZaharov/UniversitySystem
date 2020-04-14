using System.Collections.Generic;
using System.Text;

namespace UniversitySystem
{
    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }

        public List<Student> Students { get; set; }
    }
}

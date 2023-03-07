using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zirpl.Models.Courses
{
    public class Course
    {
        public Course()
        {
            Students = new List<Student>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Code { get; set; }
        public Professor? Professor { get; set; }
        public IList<Student> Students { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zirpl.Integrations.CoursesAndStudentsApiV2
{
    public class Course
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Code { get; set; }
        public string? ProfessorName { get; set; }
    }
}

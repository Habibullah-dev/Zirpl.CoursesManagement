namespace Zirpl.WebApi.Models.ApiV2.Courses
{
    public class AddCourseRequest
    {
        /// <summary>
        /// The name of the course (ex: "Introduction to Chemistry")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The course code (ex: "CHM-101")
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The course department (ex: "Chemistry")
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// The first name of the professor
        /// </summary>
        public string ProfessorFirstName { get; set; }

        /// <summary>
        /// The last name of the professor
        /// </summary>
        public string ProfessorLastName { get; set; }
    }
}

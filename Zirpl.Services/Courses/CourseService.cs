using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zirpl.Models.Courses;

namespace Zirpl.Services.Courses
{
    public class CourseService : ICourseService
    {
        public bool DoesCourseExists(int id)
        {
            return Courses.Any(o => o.Id == id);
        }

        public int GetCourseCount()
        {
            return Courses.Count;
        }

        public int AddCourse(Course course)
        {
            var id = Courses.Select(o => o.Id).Max() + 1;
            course.Id = id;
            Courses.Add(course);

            if (course.Students.Any())
            {
                var maxStudentId = Courses.SelectMany(o => o.Students).Max(o => o.Id);
                foreach (var student in course.Students)
                {
                    student.Id = ++maxStudentId;
                }
            }

            return id;
        }

        public void DeleteCourse(int id)
        {
            var course = GetCourse(id);
            Courses.Remove(course);
        }

        public Course? GetCourse(int id)
        {
            return Courses.SingleOrDefault(o => o.Id == id);
        }

        public void UpdateCourse(Course course)
        {
            var courseToUpdate = GetCourse(course.Id);
            courseToUpdate.Name = course.Name;
            courseToUpdate.Code = course.Code;
            courseToUpdate.Department = course.Department;
            courseToUpdate.Professor.FirstName = course.Professor.FirstName;
            courseToUpdate.Professor.LastName = course.Professor.LastName;
        }

        public Course?[] GetCourseList(int skip, int take, string? search)
        {
            return Courses.OrderBy(o => o.Id)
                .Where(o => string.IsNullOrWhiteSpace(search)
                || (o.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || o.Department.Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || o.Professor.FirstName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || o.Professor.LastName.Contains(search, StringComparison.InvariantCultureIgnoreCase)))
                .Skip(skip).Take(take).ToArray();
        }

        public bool DoesStudentExistInCourse(int courseId, int studentId)
        {
            return Courses.Any(o => o.Id == courseId && o.Students.Any(s => s.Id == studentId));
        }

        public int GetStudentCount(int courseId)
        {
            return Courses.Where(o => o.Id == courseId).Sum(o => o.Students.Count);
        }

        public int AddStudentToCourse(int courseId, Student student)
        {
            var course = GetCourse(courseId);
            var maxStudentId = Courses.SelectMany(o => o.Students).Max(o => o.Id);
            student.Id = maxStudentId + 1;
            course.Students.Add(student);
            return student.Id;
        }

        public void DeleteStudentFromCourse(int courseId, int studentId)
        {
            var course = GetCourse(courseId);
            var student = course.Students.Single(o => o.Id == studentId);
            course.Students.Remove(student);
        }

        public Student? GetStudentInCourse(int courseId, int studentId)
        {
            return GetCourse(courseId).Students.SingleOrDefault(o => o.Id == studentId);
        }

        public void UpdateStudentInCourse(int courseId, Student student)
        {
            var studentToUpdate = GetStudentInCourse(courseId, student.Id);
            studentToUpdate.FirstName = student.FirstName;
            studentToUpdate.LastName = student.LastName;
        }

        public Student[] GetStudentsInCourseList(int courseId, int skip, int take, string? search)
        {
            var course = GetCourse(courseId);
            return course.Students
                .Where(o => string.IsNullOrWhiteSpace(search)
                                    || o.FirstName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                                    || o.LastName.Contains(search, StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(o => o.Id).Skip(skip).Take(take).ToArray();
        }

        public void SetStudentIdentificationImage(int courseId, int studentId, byte[] image, string fileName)
        {
            var student = GetStudentInCourse(courseId, studentId);
            student.StudentIdImageFile = image;
            student.StudentIdImageFileName = fileName;
        }

        private static IList<Course?> Courses = new List<Course?>
        {
            new Course
            {
                Id = 1,
                Code = "CS-101",
                Department = "Computer Science",
                Name = "Introduction to Computer Science",
                Professor = new Professor
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    SocialSecurityNumber = "123456780"
                },
                Students = new List<Student>
                {
                    new Student
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe"
                    },
                    new Student
                    {
                        Id = 2,
                        FirstName = "Jane",
                        LastName = "Doe"
                    }
                }
            },
            new Course
            {
                Id = 2,
                Code = "BIO-101",
                Department = "Life Sciences",
                Name = "Introduction to Biology",
                Professor = new Professor
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    SocialSecurityNumber = "234567890"
                },
                Students = new List<Student>
                {
                    new Student
                    {
                        Id = 3,
                        FirstName = "Ray",
                        LastName = "Doe"
                    },
                    new Student
                    {
                        Id = 4,
                        FirstName = "Karen",
                        LastName = "Doe"
                    }
                }
            },
            new Course
            {
                Id = 3,
                Code = "ENG-101",
                Department = "Languages",
                Name = "Writing",
                Professor = new Professor
                {
                    Id = 3,
                    FirstName = "June",
                    LastName = "Smith",
                    SocialSecurityNumber = "345678900"
                },
                Students = new List<Student>
                {
                    new Student
                    {
                        Id = 5,
                        FirstName = "Jim",
                        LastName = "Doe"
                    },
                    new Student
                    {
                        Id = 6,
                        FirstName = "Erin",
                        LastName = "Doe"
                    }
                }
            }
        };
    }
}

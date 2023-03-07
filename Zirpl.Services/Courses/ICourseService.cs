using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zirpl.Models.Courses;

namespace Zirpl.Services.Courses
{
    public interface ICourseService
    {
        // Courses
        bool DoesCourseExists(int id);
        int GetCourseCount();
        int AddCourse(Course contact);
        void DeleteCourse(int id);
        Course? GetCourse(int id);
        void UpdateCourse(Course contact);
        Course?[] GetCourseList(int skip, int take, string? search);

        // Students
        bool DoesStudentExistInCourse(int courseId, int studentId);
        int GetStudentCount(int courseId);
        int AddStudentToCourse(int courseId, Student student);
        void DeleteStudentFromCourse(int courseId, int studentId);
        Student? GetStudentInCourse(int courseId, int studentId);
        void UpdateStudentInCourse(int courseId, Student student);
        Student[] GetStudentsInCourseList(int courseId, int skip, int take, string? search);
        void SetStudentIdentificationImage(int courseId, int studentId, byte[] image, string fileName);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zirpl.Integrations.CoursesAndStudentsApiV2
{
    public interface ICoursesAndStudentsApiClient
    {
        Task<Course[]> GetCourses(int? skip = null, int? take = null, string? search = null, CancellationToken? cancellationToken = null);
        Task<Course> GetCourse(int courseId, CancellationToken? cancellationToken = null);
        Task<AddCourseResponse> AddCourse(AddCourseRequest request, CancellationToken? cancellationToken = null);
        Task UpdateCourse(int courseId, UpdateCourseRequest request, CancellationToken? cancellationToken = null);
        Task DeleteCourse(int courseId, CancellationToken? cancellationToken = null);

        //Task<Student[]> GetStudents(int courseId, int? skip = null, int? take = null, 
        //    CancellationToken? cancellationToken = null);

        //Task<Student> GetStudent(int courseId, int studentId, CancellationToken? cancellationToken = null);

        //Task<AddStudentResponse> AddStudent(AddStudentRequest request, CancellationToken? cancellationToken = null);

        //Task UpdateStudent(int courseId, int studentId, UpdateStudentRequest request, 
        //    CancellationToken? cancellationToken = null);

        //Task DeleteStudent(int courseId, int studentId, CancellationToken? cancellationToken = null);

        Task SetStudentIdentificationImage(int courseId, int studentId, byte[] image, string fileName,
            CancellationToken? cancellationToken = null);
        Task DeleteStudentIdentificationImage(int courseId, int studentId, CancellationToken? cancellationToken = null);
    }
}

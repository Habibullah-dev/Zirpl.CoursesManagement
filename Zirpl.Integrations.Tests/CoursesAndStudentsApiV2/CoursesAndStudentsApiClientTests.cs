using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zirpl.Integrations.CoursesAndStudentsApiV2;

namespace Zirpl.Integrations.Tests.CoursesAndStudentsApiV2
{
    [TestClass]
    public class CoursesAndStudentsApiClientTests
    {
        private CoursesAndStudentsApiClient apiClient;

        [TestInitialize]
        public void TestInitialize()
        {
            apiClient = new CoursesAndStudentsApiClient("caller@zirpl.com", "Pass123!");
        }

        #region GetCourses

        [TestMethod]
        public async Task GetCourses_NoParameters()
        {
            var results = await apiClient.GetCourses();
            results.Should().NotBeNullOrEmpty();
            results.Length.Should().Be(3);
        }

        [TestMethod]
        public async Task GetCourses_WithSkipAndTake()
        {
            var results = await apiClient.GetCourses(1, 1);
            results.Should().NotBeNullOrEmpty();
            results.Length.Should().Be(1);
        }

        [TestMethod]
        public async Task GetCourses_WithSearch()
        {
            var results = await apiClient.GetCourses(null, null, "Computer");
            results.Should().NotBeNullOrEmpty();
            results.Length.Should().Be(1);
        }

        [TestMethod]
        public async Task GetCourses_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("badusername@zirpl.com", "badpassword");
            await new Func<Task<Course[]>>(async () =>
            {
                return await apiClient.GetCourses();
            }).Should().ThrowAsync<AuthorizationException>();
        }

        #endregion

        #region GetCourse
        
        [TestMethod]
        public async Task GetCourse_Exists()
        {
            var result = await apiClient.GetCourse(1);
            result.Should().NotBeNull();
            result.Name.Should().Be("Introduction to Computer Science");
        }

        [TestMethod]
        public async Task GetCourse_DoesNotExist()
        {
            await new Func<Task<Course>>(async () =>
            {
                return await apiClient.GetCourse(1000);
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task GetCourse_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("badusername@zirpl.com", "badpassword");
            await new Func<Task<Course>>(async () =>
            {
                return await apiClient.GetCourse(1);
            }).Should().ThrowAsync<AuthorizationException>();
        }

        #endregion

        #region Add Course

        [TestMethod]
        public async Task AddCourse_ValidInput()
        {
            var newCourse = new AddCourseRequest
            {
                Code = "CS-200",
                Name = "Intro to Computer Vision",
                Department = "Computer Science",
                ProfessorFirstName = "Joe",
                ProfessorLastName = "Black"
            };
            var response = await apiClient.AddCourse(newCourse);
            response.Should().NotBeNull();
            response.Course.Should().NotBeNull();
            response.Course.Department.Should().Be(newCourse.Department);
            response.Course.Name.Should().Be(newCourse.Name);
            response.Course.Code.Should().Be(newCourse.Code);
            response.Course.ProfessorName.Should().Be($"{newCourse.ProfessorFirstName} {newCourse.ProfessorLastName}");
            response.ResourceUri.Should().NotBeNullOrEmpty();
            response.ResourceUri.Should().Be($"https://localhost:44371/apiv2/Courses/{response.Course.Id}");
        }

        [TestMethod]
        public async Task AddCourse_InvalidInput()
        {
            var newCourse = new AddCourseRequest
            {
                Code = "CS-200",
                Name = null, // required
                Department = "Computer Science",
                ProfessorFirstName = "Joe",
                ProfessorLastName = "Black"
            };
            await new Func<Task<AddCourseResponse>>(async () =>
            {
                return await apiClient.AddCourse(newCourse);
            }).Should().ThrowAsync<ValidationException>();
        }

        [TestMethod]
        public async Task AddCourse_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("badusername@zirpl.com", "badpassword");
            await new Func<Task<AddCourseResponse>>(async () =>
            {
                var newCourse = new AddCourseRequest
                {
                    Code = "CS-200",
                    Name = "Intro to Computer Vision",
                    Department = "Computer Science",
                    ProfessorFirstName = "Joe",
                    ProfessorLastName = "Black"
                };
                return await apiClient.AddCourse(newCourse);
            }).Should().ThrowAsync<AuthorizationException>();
        }

        #endregion

        #region Update Course

        [TestMethod]
        public async Task UpdateCourse_ValidInput()
        {
            var request = new UpdateCourseRequest
            {
                Code = "CS-200",
                Name = "Intro to Computer Vision",
                Department = "Computer Science",
                ProfessorFirstName = "Joe",
                ProfessorLastName = "Black"
            };
            await apiClient.UpdateCourse(1, request);
            var course = await apiClient.GetCourse(1);
            course.Should().NotBeNull();
            course.Should().NotBeNull();
            course.Department.Should().Be(request.Department);
            course.Name.Should().Be(request.Name);
            course.Code.Should().Be(request.Code);
            course.ProfessorName.Should().Be($"{request.ProfessorFirstName} {request.ProfessorLastName}");
        }

        [TestMethod]
        public async Task UpdateCourse_CourseDoesNotExist()
        {
            await new Func<Task>(async () =>
            {
                await apiClient.UpdateCourse(1000, new UpdateCourseRequest());
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task UpdateCourse_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("badusername@zirpl.com", "badpassword");
            await new Func<Task>(async () =>
            {
                await apiClient.UpdateCourse(1, new UpdateCourseRequest());
            }).Should().ThrowAsync<AuthorizationException>();
        }

        #endregion

        #region Delete Course

        [TestMethod]
        public async Task DeleteCourse()
        {
            var newCourse = new AddCourseRequest
            {
                Code = "CS-200",
                Name = "Intro to Computer Vision",
                Department = "Computer Science",
                ProfessorFirstName = "Joe",
                ProfessorLastName = "Black"
            };
            var response = await apiClient.AddCourse(newCourse);
            await apiClient.DeleteCourse(response.Course.Id);

            await new Func<Task>(async () =>
            {
                await apiClient.DeleteCourse(response.Course.Id);
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task DeleteCourse_CourseDoesNotExist()
        {
            await new Func<Task>(async () =>
            {
                await apiClient.DeleteCourse(1000);
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task DeleteCourse_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("badusername@zirpl.com", "badpassword");
            await new Func<Task>(async () =>
            {
                await apiClient.DeleteCourse(1);
            }).Should().ThrowAsync<AuthorizationException>();
        }

        #endregion

        #region SetStudentIdentificationImage

        [TestMethod]
        public async Task SetStudentIdentificationImage_GoodImage()
        {
            byte[] image = null;
            using (var stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("Zirpl.Integrations.Tests.CoursesAndStudentsApiV2.TestImage.jpg"))
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    image = memoryStream.ToArray();
                }
            }

            await apiClient.SetStudentIdentificationImage(1, 1, image, "TestImage.jpg");
        }

        [TestMethod]
        public async Task SetStudentIdentificationImage_CourseAndStudentDoNotExist()
        {
            byte[] image = null;
            using (var stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("Zirpl.Integrations.Tests.CoursesAndStudentsApiV2.TestImage.jpg"))
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    image = memoryStream.ToArray();
                }
            }

            await new Func<Task>(async () =>
            {
                await apiClient.SetStudentIdentificationImage(1000, 1000, image, "TestImage.jpg");
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task SetStudentIdentificationImage_BadCredentials()
        {
            byte[] image = null;
            using (var stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("Zirpl.Integrations.Tests.CoursesAndStudentsApiV2.TestImage.jpg"))
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    image = memoryStream.ToArray();
                }
            }

            apiClient = new CoursesAndStudentsApiClient("badusername@zirpl.com", "badpassword");
            await new Func<Task>(async () =>
            {
                await apiClient.SetStudentIdentificationImage(1, 1, image, "TestImage.jpg");
            }).Should().ThrowAsync<AuthorizationException>();
        }

        #endregion

        #region DeleteStudentIndentificationImage

        [TestMethod]
        public async Task DeleteStudentIdentificationImage()
        {
            await apiClient.DeleteStudentIdentificationImage(1, 1);
        }

        [TestMethod]
        public async Task DeleteStudentIdentificationImage_CourseAndStudentDoNotExist()
        {
            await new Func<Task>(async () =>
            {
                await apiClient.DeleteStudentIdentificationImage(1000, 1000);
            }).Should().ThrowAsync<CourseOrStudentNotFoundException>();
        }

        [TestMethod]
        public async Task DeleteStudentIdentificationImage_BadCredentials()
        {
            apiClient = new CoursesAndStudentsApiClient("badusername@zirpl.com", "badpassword");
            await new Func<Task>(async () =>
            {
                await apiClient.DeleteStudentIdentificationImage(1, 1);
            }).Should().ThrowAsync<AuthorizationException>();
        }
        #endregion
    }
}

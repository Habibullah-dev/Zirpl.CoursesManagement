using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Zirpl.Integrations.CoursesAndStudentsApiV2
{
    public class CoursesAndStudentsApiClient : ICoursesAndStudentsApiClient
    {
        private const string baseUrl = "https://localhost:44371/apiv2/";
        private string username;
        private string password;

        public CoursesAndStudentsApiClient(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient {BaseAddress = new Uri(baseUrl)};

            var authorizationText = $"{username}:{password}";
            var authorizationBytes = System.Text.Encoding.ASCII.GetBytes(authorizationText);
            var authorizationHeaderValue = Convert.ToBase64String(authorizationBytes);
            var authenticationHeader = new AuthenticationHeaderValue("Basic", authorizationHeaderValue);
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            return httpClient;
        }

        public async Task<Course[]> GetCourses(int? skip = null, int? take = null, string? search = null, CancellationToken? cancellationToken = null)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    var url = $"courses?skip={skip}&take={take}&search={WebUtility.UrlEncode(search)}";
                    using (var responseMessage = await httpClient.GetAsync(url,
                               cancellationToken ?? CancellationToken.None))
                    {
                        if (responseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            var responseBody = await responseMessage.Content
                                .ReadAsStringAsync(cancellationToken ?? CancellationToken.None);
                            var courses = JsonSerializer.Deserialize<Course[]>(responseBody,
                                new JsonSerializerOptions(JsonSerializerDefaults.Web));
                            return courses;
                        }
                        else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            throw new AuthorizationException("Bad credentials");
                        }
                        else
                        {
                            throw new ApiException($"Unexpected http status code: {responseMessage.StatusCode}");
                        }
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ApiException("Unexpected exception", e);
            }
        }

        public async Task<Course> GetCourse(int courseId, CancellationToken? cancellationToken = null)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    var url = $"courses/{courseId}";
                    using (var responseMessage = await httpClient.GetAsync(url,
                               cancellationToken ?? CancellationToken.None))
                    {
                        if (responseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            var responseBody = await responseMessage.Content
                                .ReadAsStringAsync(cancellationToken ?? CancellationToken.None);
                            var course = JsonSerializer.Deserialize<Course>(responseBody,
                                new JsonSerializerOptions(JsonSerializerDefaults.Web));
                            return course;
                        }
                        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            throw new AuthorizationException("Bad credentials");
                        }
                        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                        {
                            throw new CourseOrStudentNotFoundException($"Course {courseId} not found");
                        }
                        throw new ApiException($"Unexpected http status code: {responseMessage.StatusCode}");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ApiException("Unexpected exception", e);
            }
        }

        public async Task<AddCourseResponse> AddCourse(AddCourseRequest request, CancellationToken? cancellationToken = null)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    var requestJson = JsonSerializer.Serialize(request);

                    using (var httpContent = new StringContent(requestJson))
                    {
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        using (var responseMessage = await httpClient.PostAsync("courses", httpContent,
                                   cancellationToken ?? CancellationToken.None))
                        {
                            if (responseMessage.StatusCode == HttpStatusCode.Created)
                            {
                                var responseBody = await responseMessage.Content
                                    .ReadAsStringAsync(cancellationToken ?? CancellationToken.None);
                                var course = JsonSerializer.Deserialize<Course>(responseBody,
                                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                                var createdAt = responseMessage.Headers.Location.AbsoluteUri;

                                return new AddCourseResponse
                                {
                                    Course = course,
                                    ResourceUri = createdAt
                                };
                            }

                            if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                throw new AuthorizationException("Bad credentials");
                            }

                            if (responseMessage.StatusCode == HttpStatusCode.UnprocessableEntity)
                            {
                                var responseBody = await responseMessage.Content
                                    .ReadAsStringAsync(cancellationToken ?? CancellationToken.None);
                                var response = JsonSerializer.Deserialize<AddCourse422Response>(responseBody,
                                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                                var errors = new List<string>();
                                if ((response?.Errors?.Code?.Any()).GetValueOrDefault())
                                {
                                    errors.AddRange(response.Errors.Code);
                                }
                                if ((response?.Errors?.Department?.Any()).GetValueOrDefault())
                                {
                                    errors.AddRange(response.Errors.Department);
                                }
                                if ((response?.Errors?.Name?.Any()).GetValueOrDefault())
                                {
                                    errors.AddRange(response.Errors.Name);
                                }
                                if ((response?.Errors?.ProfessorFirstName?.Any()).GetValueOrDefault())
                                {
                                    errors.AddRange(response.Errors.ProfessorFirstName);
                                }
                                if ((response?.Errors?.ProfessorLastName?.Any()).GetValueOrDefault())
                                {
                                    errors.AddRange(response.Errors.ProfessorLastName);
                                }

                                throw new ValidationException("Invalid input") { ValidationErrors = errors.ToArray()};
                            }

                            throw new ApiException($"Unexpected http status code: {responseMessage.StatusCode}");
                        }
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ApiException("Unexpected exception", e);
            }
        }

        public async Task UpdateCourse(int courseId, UpdateCourseRequest request, CancellationToken? cancellationToken = null)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    var requestJson = JsonSerializer.Serialize(request);

                    using (var httpContent = new StringContent(requestJson))
                    {
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var url = $"courses/{courseId}";
                        using (var responseMessage = await httpClient.PutAsync(url, httpContent,
                                   cancellationToken ?? CancellationToken.None))
                        {
                            if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                            {
                                return;
                            }
                            if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                throw new AuthorizationException("Bad credentials");
                            }
                            if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                            {
                                throw new CourseOrStudentNotFoundException($"Course {courseId} not found");
                            }
                            throw new ApiException($"Unexpected http status code: {responseMessage.StatusCode}");
                        }
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ApiException("Unexpected exception", e);
            }
        }

        public async Task DeleteCourse(int courseId, CancellationToken? cancellationToken = null)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    var url = $"courses/{courseId}";
                    using (var responseMessage = await httpClient.DeleteAsync(url,
                               cancellationToken ?? CancellationToken.None))
                    {
                        if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                        {
                            return;
                        }
                        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            throw new AuthorizationException("Bad credentials");
                        }
                        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                        {
                            throw new CourseOrStudentNotFoundException($"Course {courseId} not found");
                        }
                        throw new ApiException($"Unexpected http status code: {responseMessage.StatusCode}");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ApiException("Unexpected exception", e);
            }
        }

        public async Task SetStudentIdentificationImage(int courseId, int studentId, byte[] image, string fileName,
            CancellationToken? cancellationToken = null)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    using (var httpContent = new MultipartFormDataContent(
                               "Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        if (image != null
                            && fileName != null)
                        {
                            httpContent.Add(new StreamContent(new MemoryStream(image)), "file", fileName);
                        }

                        // DO NOT SPECIFY A MEDIA TYPE

                        var url = $"courses/{courseId}/students/{studentId}/identificationimage";
                        using (var responseMessage = await httpClient.PutAsync(url, httpContent, cancellationToken ?? CancellationToken.None))
                        {
                            if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                            {
                                return;
                            }
                            if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                throw new AuthorizationException("Bad credentials");
                            }
                            if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                            {
                                throw new CourseOrStudentNotFoundException($"Course {courseId} not found");
                            }
                            throw new ApiException($"Unexpected http status code: {responseMessage.StatusCode}");
                        }
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ApiException("Unexpected exception", e);
            }
        }

        public async Task DeleteStudentIdentificationImage(int courseId, int studentId, CancellationToken? cancellationToken = null)
        {
            try
            {
                using (var httpClient = CreateHttpClient())
                {
                    var url = $"courses/{courseId}/students/{studentId}/identificationimage";
                    using (var responseMessage = await httpClient.DeleteAsync(url,
                               cancellationToken ?? CancellationToken.None))
                    {
                        if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                        {
                            return;
                        }
                        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            throw new AuthorizationException("Bad credentials");
                        }
                        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                        {
                            throw new CourseOrStudentNotFoundException($"Course {courseId} or Student {studentId} not found");
                        }
                        throw new ApiException($"Unexpected http status code: {responseMessage.StatusCode}");
                    }
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ApiException("Unexpected exception", e);
            }
        }
    }
}

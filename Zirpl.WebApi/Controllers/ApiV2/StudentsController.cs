using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Zirpl.Models.Courses;
using Zirpl.Services.Courses;
using Zirpl.WebApi.Attributes;
using Zirpl.WebApi.Models.ApiV2.Students;

namespace Zirpl.WebApi.Controllers.ApiV2
{
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("apiv2/courses/{courseId:int}/[controller]")]
    [ApiController]
    [Produces("application/json", "application/xml")]
    [BasicAuthorization]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class StudentsController : ControllerBase
    {
        private ICourseService _courseService;
        private IMapper _mapper;

        public StudentsController(ICourseService courseService, IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a list of Students in the Course from the DataStore
        /// </summary>
        /// <param name="skip">The number of Students at the beginning of the list to skip</param>
        /// <param name="take">How many Students to include, after those that are skipped</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto[]))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult List([FromRoute] int courseId, 
            [FromQuery] int? skip, [FromQuery] int? take, [FromQuery]string? search)
        {
            if (!_courseService.DoesCourseExists(courseId))
            {
                return NotFound();
            }

            var students = _courseService.GetStudentsInCourseList(courseId, 
                skip ?? 0, take ?? 5, search);
            var studentDtos = _mapper.Map<StudentDto[]>(students);

            return Ok(studentDtos);
        }

        /// <summary>
        /// Gets a single student from a specific course
        /// </summary>
        /// <param name="courseId">the Id of the course to search</param>
        /// <param name="studentId">The id of the student to retrieve</param>
        /// <response code="404">When the course does not exist</response>
        /// <returns></returns>
        [HttpGet("{studentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get([FromRoute] int courseId, [FromRoute] int studentId)
        {
            if (!_courseService.DoesCourseExists(courseId)
                || !_courseService.DoesStudentExistInCourse(courseId, studentId))
            {
                return NotFound();
            }

            var student = _courseService.GetStudentInCourse(courseId, studentId);

            var studentDto = _mapper.Map<StudentDto>(student);

            return Ok(studentDto);
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Add([FromRoute] int courseId, [FromBody] AddStudentToCourseRequestDto student)
        {
            if (!_courseService.DoesCourseExists(courseId))
            {
                return NotFound();
            }

            var studentEntity = _mapper.Map<Student>(student);

            var studentId = _courseService.AddStudentToCourse(courseId, studentEntity);
            var addedStudent = _courseService.GetStudentInCourse(courseId, studentId);
            var addedStudentDto = _mapper.Map<StudentDto>(addedStudent);

            return CreatedAtAction(nameof(Get), 
                new { studentId = studentId, courseId = courseId }, 
                addedStudentDto);
        }

        [HttpPut("{studentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update([FromRoute] int courseId, [FromRoute] int studentId, [FromBody] UpdateStudentInCourseDto student)
        {
            if (!_courseService.DoesCourseExists(courseId)
                || !_courseService.DoesStudentExistInCourse(courseId, studentId))
            {
                return NotFound();
            }

            var studentToUpdate = _mapper.Map<Student>(student);
            studentToUpdate.Id = studentId;
            _courseService.UpdateStudentInCourse(courseId, studentToUpdate);

            return NoContent();
        }

        [HttpDelete]
        [Route("{studentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int courseId, [FromRoute] int studentId)
        {
            if (!_courseService.DoesCourseExists(courseId)
                || !_courseService.DoesStudentExistInCourse(courseId, studentId))
            {
                return NotFound();
            }

            _courseService.DeleteStudentFromCourse(courseId, studentId);

            return NoContent();
        }

        [HttpPut]
        [Route("{studentId:int}/identificationimage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetIdentificationImage([FromRoute] int courseId, [FromRoute] int studentId,
            [FromForm]IFormFile? file)
        {
            if (!_courseService.DoesCourseExists(courseId)
                || !_courseService.DoesStudentExistInCourse(courseId, studentId))
            {
                return NotFound();
            }

            if (file == null
                || file.Length > 16777216
                || file.Length == 0)
            {
                return BadRequest();
            }

            byte[]? studentImageBytes = null;
            await using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                studentImageBytes = memoryStream.ToArray();
            }

            _courseService.SetStudentIdentificationImage(courseId, studentId, studentImageBytes, file.FileName);

            return NoContent();
        }

        [HttpDelete]
        [Route("{studentId:int}/identificationimage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteIdentificationImage([FromRoute] int courseId, [FromRoute] int studentId)
        {
            if (!_courseService.DoesCourseExists(courseId)
                || !_courseService.DoesStudentExistInCourse(courseId, studentId))
            {
                return NotFound();
            }

            _courseService.SetStudentIdentificationImage(courseId, studentId, null, null);

            return NoContent();
        }
    }
}

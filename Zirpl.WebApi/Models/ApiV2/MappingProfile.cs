using AutoMapper;
using Zirpl.Models.Courses;
using Zirpl.WebApi.Models.ApiV2.Courses;
using Zirpl.WebApi.Models.ApiV2.Students;

namespace Zirpl.WebApi.Models.ApiV2
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(d => d.ProfessorName, options => options
                    .MapFrom(s => s.Professor == null ? null : $"{s.Professor.FirstName} {s.Professor.LastName}"));

            CreateMap<AddCourseRequest, Course>()
                .AfterMap(AfterMap);

            CreateMap<UpdateCourseRequest, Course>()
                .AfterMap(AfterMap);

            CreateMap<Student, StudentDto>();

            CreateMap<AddStudentToCourseRequestDto, Student>();

            CreateMap<UpdateStudentInCourseDto, Student>();
        }

        private void AfterMap(UpdateCourseRequest source, Course destination)
        {
            destination.Professor = new Professor
            {
                FirstName = source.ProfessorFirstName,
                LastName = source.ProfessorLastName
            };
        }

        private void AfterMap(AddCourseRequest source, Course destination)
        {
            destination.Professor = new Professor
            {
                FirstName = source.ProfessorFirstName,
                LastName = source.ProfessorLastName
            };
        }
    }
}

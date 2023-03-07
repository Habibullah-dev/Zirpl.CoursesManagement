namespace Zirpl.Integrations.CoursesAndStudentsApiV2;

public class UpdateCourseRequest
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Department { get; set; }
    public string ProfessorFirstName { get; set; }
    public string ProfessorLastName { get; set; }
}
namespace Zirpl.Integrations.CoursesAndStudentsApiV2;

public class AddCourse422Response
{
    public Errors Errors { get; set; }
}
public class Errors
{
    public string[] Name { get; set; }
    public string[] Code { get; set; }
    public string[] Department { get; set; }
    public string[] ProfessorFirstName { get; set; }
    public string[] ProfessorLastName { get; set; }
}
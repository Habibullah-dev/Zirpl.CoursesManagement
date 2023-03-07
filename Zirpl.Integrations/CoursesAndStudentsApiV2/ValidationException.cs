using System.Runtime.Serialization;
using System.Text;

namespace Zirpl.Integrations.CoursesAndStudentsApiV2;

[Serializable]
public class ValidationException : ApiException
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public ValidationException()
    {
    }

    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception inner) : base(message, inner)
    {
    }

    protected ValidationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    public string[] ValidationErrors { get; set; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(base.ToString());
        stringBuilder.AppendLine("Validation Errors:");
        foreach (var error in ValidationErrors)
        {
            stringBuilder.AppendLine(error);
        }

        return stringBuilder.ToString();
    }
}
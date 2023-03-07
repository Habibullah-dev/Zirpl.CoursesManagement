using FluentValidation;

namespace Zirpl.WebApi.Models.ApiV1.Courses;

public class AddCourseRequestValidator : AbstractValidator<AddCourseRequest>
{
    public AddCourseRequestValidator()
    {
        RuleFor(o => o.Name)
            .NotEmpty()
            .WithMessage("'Name' is required");
        When(o => !string.IsNullOrWhiteSpace(o.Name), () =>
        {
            RuleFor(o => o.Name)
                .MaximumLength(100)
                .WithMessage("Max length of 'Name' is 100");
        });
    }
}
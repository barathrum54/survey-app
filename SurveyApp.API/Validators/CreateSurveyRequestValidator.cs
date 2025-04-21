using FluentValidation;

namespace SurveyApp.API.DTOs
{
  public class CreateSurveyRequestValidator : AbstractValidator<CreateSurveyRequest>
  {
    public CreateSurveyRequestValidator()
    {
      RuleFor(x => x.Title)
        .NotEmpty().WithMessage("Title is required.")
        .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.");

      RuleFor(x => x.Options)
        .NotNull()
        .Must(options => options.Count >= 2 && options.Count <= 5).WithMessage("You must have between 2 and 5 options.");
    }
  }
}

using FluentValidation;

namespace SurveyApp.API.DTOs
{
  public class VoteRequestValidator : AbstractValidator<VoteRequest>
  {
    public VoteRequestValidator()
    {
      RuleFor(x => x.SurveyId)
        .GreaterThan(0).WithMessage("SurveyId is required and must be greater than 0.");

      RuleFor(x => x.OptionId)
        .GreaterThan(0).WithMessage("OptionId is required and must be greater than 0.");
    }
  }
}

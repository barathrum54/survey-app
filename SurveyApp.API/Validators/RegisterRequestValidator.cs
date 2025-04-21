using FluentValidation;

namespace SurveyApp.API.DTOs
{
  public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
  {
    public RegisterRequestValidator()
    {
      RuleFor(x => x.Username)
        .NotEmpty().WithMessage("Username is required.")
        .MinimumLength(3).WithMessage("Username should be at least 3 characters.");

      RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(6).WithMessage("Password should be at least 6 characters.");

      RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Please provide a valid email address.");
    }
  }
}

using System.ComponentModel.DataAnnotations;

namespace SurveyApp.API.DTOs;

public class CreateSurveyRequest
{
  [Required, MinLength(3), MaxLength(100)]
  public string Title { get; set; } = string.Empty;

  [Required]
  [MinLength(2, ErrorMessage = "At least 2 options are required.")]
  [MaxLength(5, ErrorMessage = "At most 5 options are allowed.")]
  public List<string> Options { get; set; } = [];
}

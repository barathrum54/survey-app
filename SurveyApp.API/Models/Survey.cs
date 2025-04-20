namespace SurveyApp.API.Models;

public class Survey
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public int CreatedBy { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

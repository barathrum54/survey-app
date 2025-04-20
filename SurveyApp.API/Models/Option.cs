namespace SurveyApp.API.Models;

public class Option
{
  public int Id { get; set; }
  public string Text { get; set; } = string.Empty;
  public int SurveyId { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

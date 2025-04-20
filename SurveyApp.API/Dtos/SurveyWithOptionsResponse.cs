namespace SurveyApp.API.DTOs;

public class SurveyWithOptionsResponse
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public int CreatedBy { get; set; }
  public DateTime CreatedAt { get; set; }
  public List<OptionResponse> Options { get; set; } = [];
}

public class OptionResponse
{
  public int Id { get; set; }
  public string Text { get; set; } = string.Empty;
  public int SurveyId { get; set; }
}

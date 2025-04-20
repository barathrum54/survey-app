namespace SurveyApp.API.Models
{
  public class Vote
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SurveyId { get; set; }
    public int OptionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
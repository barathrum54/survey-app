using SurveyApp.API.Models;

namespace SurveyApp.API.Services.Interfaces
{
  public interface IVoteService
  {
    Vote AddVote(int userId, int surveyId, int optionId);
    IEnumerable<Vote> GetVotesBySurveyId(int surveyId);
  }
}

using SurveyApp.API.Models;

namespace SurveyApp.API.DAOs.Interfaces
{
  public interface IVoteDao
  {
    Vote Insert(Vote vote);
    Vote? GetByUserAndSurvey(int userId, int surveyId);
    IEnumerable<Vote> GetVotesBySurveyId(int surveyId);
  }
}

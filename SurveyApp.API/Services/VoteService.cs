using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Models;
using SurveyApp.API.Services.Interfaces;

namespace SurveyApp.API.Services
{
  public class VoteService : IVoteService
  {
    private readonly IVoteDao _voteDao;

    public VoteService(IVoteDao voteDao)
    {
      _voteDao = voteDao;
    }

    public Vote AddVote(int userId, int surveyId, int optionId)
    {
      // Check if user has already voted on this survey
      var existingVote = _voteDao.GetByUserAndSurvey(userId, surveyId);
      if (existingVote != null)
        throw new InvalidOperationException("User has already voted on this survey.");

      // Create the vote
      var vote = new Vote
      {
        UserId = userId,
        SurveyId = surveyId,
        OptionId = optionId
      };

      return _voteDao.Insert(vote);
    }

    public IEnumerable<Vote> GetVotesBySurveyId(int surveyId)
    {
      return _voteDao.GetVotesBySurveyId(surveyId);
    }
  }
}

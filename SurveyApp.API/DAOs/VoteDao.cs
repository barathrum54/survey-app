using SqlBatis.DataMapper;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Models;

namespace SurveyApp.API.DAOs
{
  public class VoteDao : IVoteDao
  {
    private readonly ISqlMapper _mapper;

    public VoteDao(ISqlMapper mapper)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public Vote Insert(Vote vote)
    {
      try
      {
        _mapper.Insert("Votes.Insert", vote);
        return vote;
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to insert vote: " + ex.Message, ex);
      }
    }

    public Vote? GetByUserAndSurvey(int userId, int surveyId)
    {
      return _mapper.QueryForObject<Vote>("Votes.GetByUserAndSurvey", new { UserId = userId, SurveyId = surveyId });
    }

    public IEnumerable<Vote> GetVotesBySurveyId(int surveyId)
    {
      return _mapper.QueryForList<Vote>("Votes.GetVotesBySurveyId", surveyId);
    }
  }
}
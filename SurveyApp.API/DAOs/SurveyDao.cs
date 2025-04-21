using SqlBatis.DataMapper;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Models;

namespace SurveyApp.API.DAOs;

public class SurveyDao : ISurveyDao
{
  private readonly ISqlMapper _mapper;

  public SurveyDao(ISqlMapper mapper)
  {
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
  }
  public Survey Insert(Survey survey)
  {
    try
    {
      _mapper.Insert("Surveys.Insert", survey);
      var inserted = _mapper.QueryForObject<Survey>("Surveys.GetByTitle", survey.Title); // 🔁 fallback
      if (inserted == null) throw new Exception("Failed to retrieve inserted record");
      survey.Id = inserted.Id;

      return survey;
    }
    catch (Exception ex)
    {
      throw new Exception("Failed to insert survey: " + ex.Message, ex);
    }
  }

  public Survey? GetById(int id)
  {
    return _mapper.QueryForObject<Survey>("Surveys.GetById", id);
  }

  public IEnumerable<Survey> GetAll()
  {
    return _mapper.QueryForList<Survey>("Surveys.GetAll", null);
  }
  public IEnumerable<Survey> GetByUserId(int userId)
  {
    return _mapper.QueryForList<Survey>("Surveys.GetByUserId", userId);
  }
  public void DeleteById(int id)
  {
    _mapper.Delete("Surveys.DeleteById", id);
  }
}
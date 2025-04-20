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
    Console.WriteLine("📥 DAO Insert START for " + survey.Title);
    try
    {
      _mapper.Insert("Surveys.Insert", survey);
      var inserted = _mapper.QueryForObject<Survey>("Surveys.GetByTitle", survey.Title); // 🔁 fallback
      if (inserted == null) throw new Exception("Failed to retrieve inserted record");
      survey.Id = inserted.Id;

      Console.WriteLine("✅ DAO Insert DONE with ID: " + survey.Id);
      return survey;
    }
    catch (Exception ex)
    {
      Console.WriteLine("🔥 DAO ERROR: " + ex.Message);
      Console.WriteLine("🔥 STACK: " + ex.StackTrace);
      throw;
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
    Console.WriteLine($"📤 Deleting Survey with ID: {id}");
    _mapper.Delete("Surveys.DeleteById", id);
  }
}
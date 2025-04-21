using SqlBatis.DataMapper;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Models;

namespace SurveyApp.API.DAOs;

public class OptionDao : IOptionDao
{
  private readonly ISqlMapper _mapper;

  public OptionDao(ISqlMapper mapper)
  {
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
  }
  public Option Insert(Option option)
  {
    try
    {
      _mapper.Insert("Options.Insert", option);

      var inserted = _mapper.QueryForObject<Option>("Options.GetByText", option.Text);
      if (inserted == null)
        throw new Exception("Failed to retrieve inserted record");

      option.Id = inserted.Id;

      return option;
    }
    catch (Exception ex)
    {
      throw new Exception("Failed to insert option: " + ex.Message, ex);
    }
  }

  public Option? GetById(int id)
  {
    return _mapper.QueryForObject<Option>("Options.GetById", id);
  }

  public IEnumerable<Option> GetAll()
  {
    return _mapper.QueryForList<Option>("Options.GetAll", null);
  }
  public IEnumerable<Option> GetBySurveyId(int surveyId)
  {
    return _mapper.QueryForList<Option>("Options.GetBySurveyId", surveyId);
  }
}
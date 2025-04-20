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
    Console.WriteLine("📥 DAO Insert START for " + option.Text);
    try
    {
      _mapper.Insert("Options.Insert", option);

      var inserted = _mapper.QueryForObject<Option>("Options.GetByText", option.Text);
      if (inserted == null)
        throw new Exception("Failed to retrieve inserted record");

      option.Id = inserted.Id;

      Console.WriteLine("✅ DAO Insert DONE with ID: " + option.Id);
      return option;
    }
    catch (Exception ex)
    {
      Console.WriteLine("🔥 DAO ERROR: " + ex.Message);
      Console.WriteLine("🔥 STACK: " + ex.StackTrace);
      throw;
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
}
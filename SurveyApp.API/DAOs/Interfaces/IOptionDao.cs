using SurveyApp.API.Models;

namespace SurveyApp.API.DAOs.Interfaces;

public interface IOptionDao
{
  Option Insert(Option option);
  Option? GetById(int id);
  IEnumerable<Option> GetAll();
}

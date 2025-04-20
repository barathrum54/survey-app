using SurveyApp.API.Models;

namespace SurveyApp.API.DAOs.Interfaces;

public interface ISurveyDao
{
  Survey Insert(Survey survey);
  Survey? GetById(int id);
  IEnumerable<Survey> GetAll();
}

namespace SurveyApp.API.DAOs.Interfaces;

using SurveyApp.API.Models;

public interface IUserDao
{
  User? GetByUsername(string username);
}

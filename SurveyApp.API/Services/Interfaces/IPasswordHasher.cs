namespace SurveyApp.API.Services.Interfaces;

public interface IPasswordHasher
{
  string Hash(string password);
}
namespace SurveyApp.API.Services.Interfaces;

public interface IJwtTokenService
{
  string GenerateToken(int userId, string username);
}

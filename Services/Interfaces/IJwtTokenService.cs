namespace SurveyApp.API.Services.Interfaces;

public interface IJwtTokenService
{
  string GenerateToken(Guid userId, string username);
}

using SurveyApp.API.DTOs;

namespace SurveyApp.API.Services.Interfaces;

public interface ISurveyService
{
  SurveyWithOptionsResponse CreateSurvey(CreateSurveyRequest request, int userId);
  SurveyWithOptionsResponse? GetSurveyById(int id);
  IEnumerable<SurveyWithOptionsResponse> GetSurveysByUserId(int userId);
  void DeleteSurvey(int id, int userId);
}
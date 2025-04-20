using SurveyApp.API.DTOs;
using SurveyApp.API.Models;

namespace SurveyApp.API.Services.Interfaces;

public interface ISurveyService
{
  Survey CreateSurvey(CreateSurveyRequest request, int userId);
  SurveyWithOptionsResponse? GetSurveyById(int id);

}

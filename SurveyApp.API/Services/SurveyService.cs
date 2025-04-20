using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.DTOs;
using SurveyApp.API.Models;
using SurveyApp.API.Services.Interfaces;

namespace SurveyApp.API.Services;

public class SurveyService : ISurveyService
{
  private readonly ISurveyDao _surveyDao;
  private readonly IOptionDao _optionDao;

  public SurveyService(ISurveyDao surveyDao, IOptionDao optionDao)
  {
    _surveyDao = surveyDao;
    _optionDao = optionDao;
  }

  public Survey CreateSurvey(CreateSurveyRequest request, int userId)
  {
    if (request.Options == null || request.Options.Count < 2 || request.Options.Count > 5)
      throw new ArgumentException("Survey must contain between 2 and 5 options.");

    var survey = new Survey
    {
      Title = request.Title.Trim(),
      CreatedBy = userId,
      CreatedAt = DateTime.UtcNow
    };

    var createdSurvey = _surveyDao.Insert(survey);

    foreach (var optionText in request.Options)
    {
      var option = new Option
      {
        SurveyId = createdSurvey.Id,
        Text = optionText.Trim(),
        CreatedAt = DateTime.UtcNow
      };
      _optionDao.Insert(option);
    }

    return createdSurvey;
  }

}
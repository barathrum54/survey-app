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

  public SurveyWithOptionsResponse CreateSurvey(CreateSurveyRequest request, int userId)
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

    var optionResponses = new List<OptionResponse>();
    foreach (var optionText in request.Options)
    {
      var option = new Option
      {
        SurveyId = createdSurvey.Id,
        Text = optionText.Trim(),
        CreatedAt = DateTime.UtcNow
      };
      var inserted = _optionDao.Insert(option);
      optionResponses.Add(new OptionResponse
      {
        Id = inserted.Id,
        Text = inserted.Text,
        SurveyId = inserted.SurveyId
      });
    }

    return new SurveyWithOptionsResponse
    {
      Id = createdSurvey.Id,
      Title = createdSurvey.Title,
      CreatedBy = createdSurvey.CreatedBy,
      CreatedAt = createdSurvey.CreatedAt,
      Options = optionResponses
    };
  }
  public SurveyWithOptionsResponse? GetSurveyById(int id)
  {
    var survey = _surveyDao.GetById(id);
    if (survey == null) return null;

    var options = _optionDao.GetBySurveyId(id);
    return new SurveyWithOptionsResponse
    {
      Id = survey.Id,
      Title = survey.Title,
      CreatedBy = survey.CreatedBy,
      CreatedAt = survey.CreatedAt,
      Options = options.Select(o => new OptionResponse
      {
        Id = o.Id,
        Text = o.Text,
        SurveyId = o.SurveyId
      }).ToList()
    };
  }
  public IEnumerable<SurveyWithOptionsResponse> GetSurveysByUserId(int userId)
  {
    var surveys = _surveyDao.GetByUserId(userId);
    return surveys.Select(survey => new SurveyWithOptionsResponse
    {
      Id = survey.Id,
      Title = survey.Title,
      CreatedBy = survey.CreatedBy,
      CreatedAt = survey.CreatedAt,
      Options = _optionDao.GetBySurveyId(survey.Id)
        .Select(o => new OptionResponse
        {
          Id = o.Id,
          Text = o.Text,
          SurveyId = o.SurveyId
        }).ToList()
    });
  }
  public void DeleteSurvey(int id, int userId)
  {
    var survey = _surveyDao.GetById(id);
    if (survey == null)
      throw new KeyNotFoundException("Survey not found.");
    if (survey.CreatedBy != userId)
      throw new UnauthorizedAccessException("Not allowed to delete this survey.");

    _surveyDao.DeleteById(id);
  }


}
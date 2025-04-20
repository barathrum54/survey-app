using SurveyApp.API.DAOs;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Models;
using SurveyApp.API.Tests.Fixtures;
using Xunit;

namespace SurveyApp.API.Tests.DAOs;

public class SurveyDaoTests : IClassFixture<DatabaseFixture>
{
  private readonly ISurveyDao _surveyDao;

  public SurveyDaoTests(DatabaseFixture fixture)
  {
    _surveyDao = fixture.GetService<ISurveyDao>(); // no cast
  }
  [Fact]
  public void Insert_And_GetById_ShouldWorkCorrectly()
  {
    var survey = new Survey
    {
      Title = "Test Survey",
      CreatedBy = 1,
      CreatedAt = DateTime.UtcNow
    };

    _surveyDao.Insert(survey);
    var result = _surveyDao.GetById(survey.Id);

    Assert.NotNull(result);
    Assert.Equal(survey.Title, result?.Title);
    Assert.Equal(survey.CreatedBy, result?.CreatedBy);
  }
}
